var material;

function resourceUrl(folder, filename) {
    return `/resources/${folder}/${filename}`;
}

function createMaterial() {
    if (material === undefined) {
        const textureLoader = new THREE.TextureLoader();
        material = new THREE.MeshPhongMaterial({
            map: textureLoader.load(resourceUrl("textures", "tile.png")),
            bumpMap: textureLoader.load(resourceUrl("bumpmaps", "tile.png")),
            bumpScale: 0.2
        });
    }
}

function removeMeshes(scene) {
    for (let k = scene.children.length - 1; k >= 0; k--) {
        const child = scene.children[k];
        if (child.type === "Mesh") {
            scene.remove(child);
        }
    }
}

function getGeometryPromise() {
    return new Promise((resolve, reject) => {
        var jsonloader = new THREE.JSONLoader();
        jsonloader.load(resourceUrl("geometries", "tile.json"),
            function (g) {
                resolve(g);
            }
        );
    });
}

function createCamera(width, height) {
    const tempHeight = 400;
    const viewAngle = 45;
    const near = 0.1;
    const far = 10000;
    const tanFov = Math.tan(((Math.PI / 180) * viewAngle / 2));
    const fov = (360 / Math.PI) * Math.atan(tanFov * (height / tempHeight));
    const cameraAspect = width / height;

    const camera = new THREE.PerspectiveCamera(fov, cameraAspect, near, far);
    camera.position.x = 0;
    camera.position.y = 0;
    camera.position.z = 15;
    camera.lookAt(new THREE.Vector3(0, 0, 0));
    return camera;
}

var renderContexts = [];

function RenderContext(canvasName) {
    const canvas = document.querySelector("#" + canvasName);
    const displayWidth = canvas.clientWidth;
    const displayHeight = canvas.clientHeight;
    this.camera = createCamera(displayWidth, displayHeight);
    this.scene = new THREE.Scene();
    this.renderer = new THREE.WebGLRenderer({ canvas: canvas, antialias: true });
    this.renderer.setSize(displayWidth, displayHeight);
    renderContexts.push(this);
}
RenderContext.prototype.render = function () { this.renderer.render(this.scene, this.camera); };
RenderContext.prototype.clearMeshes = function () { removeMeshes(this.scene); };

function createTiles(context, arrange) {
    context.clearMeshes();
    createMaterial();

    if (context.geometry === undefined) {
        getGeometryPromise().then(g => {
            context.geometry = g;
            arrange();
            context.render();
        });
    } else {
        arrange();
        context.render();
    }
}

function initThreeJS() {
    THREE.DefaultLoadingManager.onLoad = function () { renderContexts.forEach(r => r.render()); };
}