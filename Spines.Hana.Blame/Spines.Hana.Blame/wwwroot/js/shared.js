var font;
var renderContexts = [];

var _staticTileData = {
    uvs: [new Array(10), new Array(10), new Array(10), new Array(10)],
    geometries: [new Array(10), new Array(10), new Array(10), new Array(10)]
};

function RenderContext(canvasName) {
    const canvas = document.querySelector(`#${canvasName}`);
    const displayWidth = canvas.clientWidth;
    const displayHeight = canvas.clientHeight;
    this.camera = createCamera(displayWidth, displayHeight);
    this.scene = new THREE.Scene();
    this.renderer = new THREE.WebGLRenderer({ canvas: canvas, antialias: true });
    this.renderer.setSize(displayWidth, displayHeight);
    this.meshes = [];
    this.tiles = [];
    renderContexts.push(this);
}

RenderContext.prototype.render = function() {
    this.renderer.render(this.scene, this.camera);
};

RenderContext.prototype.createTiles = function(arrange) {
    this.clear();
    createMaterial();

    if (this.geometry === undefined) {
        getGeometryPromise().then(g => {
            this.geometry = g;
            arrange();
            this.render();
        });
    } else {
        arrange();
        this.render();
    }
}

RenderContext.prototype.createTileMesh = function (number, suit) {
    const geometry = this._getGeometry(number, suit);
    return new THREE.Mesh(geometry, _staticTileData.material);
}

RenderContext.prototype.createGhostTileMesh = function (number, suit) {
    const geometry = this._getGeometry(number, suit);
    return new THREE.Mesh(geometry, _staticTileData.ghostMaterial);
}

RenderContext.prototype.setCameraPosition = function (x, y, z) {
    this.camera.position.x = x;
    this.camera.position.y = y;
    this.camera.position.z = z;
    this.camera.lookAt(new THREE.Vector3(0, 0, 0));
}

RenderContext.prototype.addMesh = function(mesh, disposeOnClear) {
    this.scene.add(mesh);
    this.meshes.push({ mesh: mesh, disposeOnClear: disposeOnClear });
}

RenderContext.prototype.addTile = function(mesh) {
    this.scene.add(mesh);
    this.tiles.push(mesh);
}

RenderContext.prototype.clear = function () {
    while (this.meshes.length > 0) {
        const tuple = this.meshes.pop();
        const mesh = tuple.mesh;
        this.scene.remove(mesh);
        if (tuple.disposeOnClear) {
            mesh.material.dispose();
            mesh.geometry.dispose();
        }
    }
    while (this.tiles.length > 0) {
        const mesh = this.tiles.pop();
        this.scene.remove(mesh);
    }
}

function initThreeJS() {
    const fontLoader = new THREE.FontLoader();
    const url = resourceUrl("fonts", "helvetiker_bold.typeface.json");
    fontLoader.load(url,
        function (f) {
            font = f;
        }
    );

    THREE.DefaultLoadingManager.onLoad = function () {
        const count = renderContexts.length;
        for (let i = 0; i < count; i++) {
            renderContexts[i].render();
        }
    };
}

function resourceUrl(folder, filename) {
    return `/resources/${folder}/${filename}`;
}

function createMaterial() {
    if (_staticTileData.material === undefined) {
        const textureLoader = new THREE.TextureLoader();
        const texture = textureLoader.load(resourceUrl("textures", "tile.png"));
        const bump = textureLoader.load(resourceUrl("bumpmaps", "tile.png"));
        _staticTileData.material = new THREE.MeshPhongMaterial({
            map: texture,
            bumpMap: bump,
            bumpScale: 0.2
        });
        _staticTileData.ghostMaterial = new THREE.MeshPhongMaterial({
            map: texture,
            bumpMap: bump,
            bumpScale: 0.2,
            color: new THREE.Color(0x888888)
        });
    }
}

function getGeometryPromise() {
    return new Promise(resolve => {
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
    const viewAngle = 30;
    const near = 0.1;
    const far = 10000;
    const tanFov = Math.tan(((Math.PI / 180) * viewAngle / 2));
    const fov = (360 / Math.PI) * Math.atan(tanFov * (height / tempHeight));
    const cameraAspect = width / height;
    return new THREE.PerspectiveCamera(fov, cameraAspect, near, far);
}

RenderContext.prototype._getGeometry = function (number, suit) {
    if (_staticTileData.geometries[suit][number] === undefined) {
        const g = new THREE.Geometry();
        g.boundingBox = this.geometry.boundingBox;
        g.boundingSphere = this.geometry.boundingSphere;
        g.colors = this.geometry.colors;
        g.colorsNeedUpdate = this.geometry.colorsNeedUpdate;
        g.elementsNeedUpdate = this.geometry.elementsNeedUpdate;
        //g.faceVertexUvs = this.geometry.faceVertexUvs;
        g.faces = this.geometry.faces;
        g.groupsNeedUpdate = this.geometry.groupsNeedUpdate;
        g.lineDistances = this.geometry.lineDistances;
        g.morphNormals = this.geometry.morphNormals;
        g.morphTargets = this.geometry.morphTargets;
        g.name = this.geometry.name;
        g.normalsNeedUpdate = this.geometry.normalsNeedUpdate;
        g.skinIndices = this.geometry.skinIndices;
        g.skinWeights = this.geometry.skinWeights;
        g.type = this.geometry.type;
        //g.uuid = this.geometry.uuid;
        g.uvsNeedUpdate = this.geometry.uvsNeedUpdate;
        g.vertices = this.geometry.vertices;
        g.verticesNeedUpdate = this.geometry.verticesNeedUpdate;
        //g.id = this.geometry.id;

        g.faceVertexUvs = this._getUvs(suit, number);

        _staticTileData.geometries[suit][number] = g;
    }

    return _staticTileData.geometries[suit][number];
}

RenderContext.prototype._getUvs = function (suit, number) {
    if (_staticTileData.uvs[suit][number] === undefined) {
        const left = (100 + number * 32) / 512;
        const right = (100 + 24 + number * 32) / 512;
        const top = (512 - 32 - 64 * suit) / 512;
        const bottom = (512 - 32 - 32 - 64 * suit) / 512;
        const a = new THREE.Vector2(right, bottom);
        const b = new THREE.Vector2(left, top);
        const c = new THREE.Vector2(left, bottom);
        const d = new THREE.Vector2(right, top);

        const uvs = this.geometry.faceVertexUvs.slice(0);
        uvs[0] = uvs[0].slice(0);
        uvs[0][3] = [a, b, c];
        uvs[0][153] = [a, d, b];
        _staticTileData.uvs[suit][number] = uvs;
    }
    return _staticTileData.uvs[suit][number];
}
