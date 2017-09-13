var font;
var renderContexts = [];

var _staticDisplayData = {
    tileUvs: [new Array(10), new Array(10), new Array(10), new Array(10)],
    tileGeometries: [new Array(10), new Array(10), new Array(10), new Array(10)],
    baUvs: new Array(10),
    baGeometries: new Array(10)
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
    this.ba = [];
    renderContexts.push(this);
}

RenderContext.prototype.createAmbientLight = function (color) {
    const light = new THREE.AmbientLight(color);
    this.scene.add(light);
};

RenderContext.prototype.createPointLight = function (color, x, y, z) {
    const light = new THREE.PointLight(color);
    light.position.x = x;
    light.position.y = y;
    light.position.z = z;
    this.scene.add(light);
};

RenderContext.prototype.render = function() {
    this.renderer.render(this.scene, this.camera);
};

RenderContext.prototype.createTiles = function(arrange) {
    this.clear();
    createMaterial();

    if (this.tileGeometry === undefined) {
        Promise.all([getGeometryPromise("tile"), getGeometryPromise("ba")]).then(geometries => {
            [this.tileGeometry, this.baGeometry] = geometries;
            arrange();
            this.render();
        });
    } else {
        arrange();
        this.render();
    }
}

RenderContext.prototype.createBaMesh = function (value) {
    const index = [100, 1000, 5000, 10000].indexOf(value);
    const geometry = this._getBaGeometry(index);
    return new THREE.Mesh(geometry, _staticDisplayData.material);
}

RenderContext.prototype.createTileMesh = function (number, suit) {
    const geometry = this._getTileGeometry(number, suit);
    return new THREE.Mesh(geometry, _staticDisplayData.material);
}

RenderContext.prototype.createGhostTileMesh = function (number, suit) {
    const geometry = this._getTileGeometry(number, suit);
    return new THREE.Mesh(geometry, _staticDisplayData.ghostMaterial);
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

RenderContext.prototype.addBa = function (mesh) {
    this.scene.add(mesh);
    this.ba.push(mesh);
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
    while (this.ba.length > 0) {
        const mesh = this.ba.pop();
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
    if (_staticDisplayData.material === undefined) {
        const textureLoader = new THREE.TextureLoader();
        const texture = textureLoader.load(resourceUrl("textures", "tile.png"));
        const bump = textureLoader.load(resourceUrl("bumpmaps", "tile.png"));
        _staticDisplayData.material = new THREE.MeshPhongMaterial({
            map: texture,
            bumpMap: bump,
            bumpScale: 0.2
        });
        _staticDisplayData.ghostMaterial = new THREE.MeshPhongMaterial({
            map: texture,
            bumpMap: bump,
            bumpScale: 0.2,
            color: new THREE.Color(0x888888)
        });
    }
}

function getGeometryPromise(name) {
    return new Promise(resolve => {
        var jsonloader = new THREE.JSONLoader();
        jsonloader.load(resourceUrl("geometries", name + ".json"),
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

RenderContext.prototype._getTileGeometry = function (number, suit) {
    if (_staticDisplayData.tileGeometries[suit][number] === undefined) {
        const g = this._cloneGeometry(this.tileGeometry);
        g.faceVertexUvs = this._getTileUvs(suit, number);
        _staticDisplayData.tileGeometries[suit][number] = g;
    }
    return _staticDisplayData.tileGeometries[suit][number];
}

RenderContext.prototype._getBaGeometry = function (index) {
    if (_staticDisplayData.baGeometries[index] === undefined) {
        const g = this._cloneGeometry(this.baGeometry);
        g.faceVertexUvs = this._getBaUvs(index);
        _staticDisplayData.baGeometries[index] = g;
    }
    return _staticDisplayData.baGeometries[index];
}

RenderContext.prototype._cloneGeometry = function (source) {
    const g = new THREE.Geometry();
    g.boundingBox = source.boundingBox;
    g.boundingSphere = source.boundingSphere;
    g.colors = source.colors;
    g.colorsNeedUpdate = source.colorsNeedUpdate;
    g.elementsNeedUpdate = source.elementsNeedUpdate;
    //g.faceVertexUvs = source.faceVertexUvs;
    g.faces = source.faces;
    g.groupsNeedUpdate = source.groupsNeedUpdate;
    g.lineDistances = source.lineDistances;
    g.morphNormals = source.morphNormals;
    g.morphTargets = source.morphTargets;
    g.name = source.name;
    g.normalsNeedUpdate = source.normalsNeedUpdate;
    g.skinIndices = source.skinIndices;
    g.skinWeights = source.skinWeights;
    g.type = source.type;
    //g.uuid = source.uuid;
    g.uvsNeedUpdate = source.uvsNeedUpdate;
    g.vertices = source.vertices;
    g.verticesNeedUpdate = source.verticesNeedUpdate;
    //g.id = source.id;
    return g;
}

RenderContext.prototype._getBaUvs = function (index) {
    if (_staticDisplayData.baUvs[index] === undefined) {
        const delta = (32 * index) / 512;

        const uvs = this.baGeometry.faceVertexUvs.slice(0);
        uvs[0] = uvs[0].slice(0);
        const length = uvs[0].length;
        for (let i = 0; i < length; i++) {
            const source = uvs[0][i];
            const a = new THREE.Vector2(source[0].x, source[0].y + delta);
            const b = new THREE.Vector2(source[1].x, source[1].y + delta);
            const c = new THREE.Vector2(source[2].x, source[2].y + delta);
            uvs[0][i] = [a, b, c];
        }
        _staticDisplayData.baUvs[index] = uvs;
    }
    return _staticDisplayData.baUvs[index];
}

RenderContext.prototype._getTileUvs = function (suit, number) {
    if (_staticDisplayData.tileUvs[suit][number] === undefined) {
        const left = (100 + number * 32) / 512;
        const right = (100 + 24 + number * 32) / 512;
        const top = (512 - 32 - 64 * suit) / 512;
        const bottom = (512 - 32 - 32 - 64 * suit) / 512;
        const a = new THREE.Vector2(right, bottom);
        const b = new THREE.Vector2(left, top);
        const c = new THREE.Vector2(left, bottom);
        const d = new THREE.Vector2(right, top);

        const uvs = this.tileGeometry.faceVertexUvs.slice(0);
        uvs[0] = uvs[0].slice(0);
        uvs[0][3] = [a, b, c];
        uvs[0][153] = [a, d, b];
        _staticDisplayData.tileUvs[suit][number] = uvs;
    }
    return _staticDisplayData.tileUvs[suit][number];
}
