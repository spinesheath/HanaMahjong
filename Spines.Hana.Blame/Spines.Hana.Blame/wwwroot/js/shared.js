var material;

function resourceUrl(folder, filename) {
    return "/resources/" + folder + "/" + filename;
}

function createMaterial() {
    if (material === undefined) {
        var textureLoader = new THREE.TextureLoader();
        material = new THREE.MeshPhongMaterial({
            map: textureLoader.load(resourceUrl("textures", "tile.png")),
            bumpMap: textureLoader.load(resourceUrl("bumpmaps", "tile.png")),
            bumpScale: 0.2
        });
    }
}

function removeMeshes(scene) {
    for (var k = scene.children.length - 1; k >= 0; k--) {
        var child = scene.children[k];
        if (child.type === "Mesh") {
            scene.remove(child);
        }
    }
}

function createCamera(width, height) {
    var tempHeight = 400;
    var viewAngle = 45;
    var near = 0.1;
    var far = 10000;
    var tanFov = Math.tan(((Math.PI / 180) * viewAngle / 2));
    var fov = (360 / Math.PI) * Math.atan(tanFov * (height / tempHeight));
    var cameraAspect = width / height;

    var camera = new THREE.PerspectiveCamera(fov, cameraAspect, near, far);
    camera.position.x = 0;
    camera.position.y = 0;
    camera.position.z = 15;
    camera.lookAt(new THREE.Vector3(0, 0, 0));
    return camera;
}

var renderContexts = [];

function RenderContext(canvasName) {
    var canvas = document.querySelector("#" + canvasName);
    var displayWidth = canvas.clientWidth;
    var displayHeight = canvas.clientHeight;
    this.camera = createCamera(displayWidth, displayHeight);
    this.scene = new THREE.Scene();
    this.renderer = new THREE.WebGLRenderer({ canvas: canvas, antialias: true });
    this.renderer.setSize(displayWidth, displayHeight);
    renderContexts.push(this);
}
RenderContext.prototype.render = function () { this.renderer.render(this.scene, this.camera); };
RenderContext.prototype.clearMeshes = function () { removeMeshes(this.scene); };

function initThreeJS() {
    THREE.DefaultLoadingManager.onLoad = function () { renderContexts.forEach(r => r.render()); };
}