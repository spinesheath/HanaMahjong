function changeUrl(val) {
    document.location = "//" + location.host + location.pathname + "?h=" + val;
}

function renderWwyd() {
    var displayHeight = 100;
    var displayWidth = 600;
    var tileTypes = [
        "1p",
        "2p",
        "3p",
        "2m",
        "3m",
        "4m",
        "0s",
        "5s",
        "5s",
        "1z",
        "2z",
        "4z",
        "7z",
        "6z"
    ];

    var container = document.querySelector("#container");
    var renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setSize(displayWidth, displayHeight);
    container.append(renderer.domElement);

    var camera = createCamera(displayWidth, displayHeight);
    var scene = new THREE.Scene();

    THREE.DefaultLoadingManager.onLoad = function () {
        renderer.render(scene, camera);
    };

    //THREE.DefaultLoadingManager.onProgress = function (item, loaded, total) {
    //    if (loaded === total)
    //        renderer.render(scene, camera);
    //};

    createLights(scene);
    createTiles(scene, renderer, camera, tileTypes);
}

function createLights(scene) {
    var light = new THREE.AmbientLight(0x999999);
    scene.add(light);
    var pointLight = new THREE.PointLight(0x555555);
    pointLight.position.x = 100;
    pointLight.position.y = 100;
    pointLight.position.z = 700;
    scene.add(pointLight);
}

function createTiles(scene, renderer, camera, tileTypes) {

    var textureLoader = new THREE.TextureLoader();
    var material = new THREE.MeshPhongMaterial({
        map: textureLoader.load(resourceUrl("textures", "tile.png")),
        bumpMap: textureLoader.load(resourceUrl("bumpmaps", "tile.png")),
        bumpScale: 0.2
    });

    var jsonloader = new THREE.JSONLoader();
    jsonloader.load(resourceUrl("geometries", "tile.json"),
        function (geometry) {

            var meshes = [];
            for (var i = 0; i < tileTypes.length && i < 14; i++) {
                var tileType = tileTypes[i];
                var x = tileType[0]; // number
                var y = "mpsz".indexOf(tileType[1]); // suit
                var left = (100 + x * 32) / 512;
                var right = (100 + 24 + x * 32) / 512;
                var top = (512 - 32 - 64 * y) / 512;
                var bottom = (512 - 32 - 32 - 64 * y) / 512;
                var a = new THREE.Vector2(right, bottom);
                var b = new THREE.Vector2(left, top);
                var c = new THREE.Vector2(left, bottom);
                var d = new THREE.Vector2(right, top);

                var geometryClone = geometry.clone();
                geometryClone.faceVertexUvs[0][3][0] = a;
                geometryClone.faceVertexUvs[0][3][1] = b;
                geometryClone.faceVertexUvs[0][3][2] = c;
                geometryClone.faceVertexUvs[0][153][0] = a;
                geometryClone.faceVertexUvs[0][153][1] = d;
                geometryClone.faceVertexUvs[0][153][2] = b;

                var tileWidth = 0.97;
                var mesh = new THREE.Mesh(geometryClone, material);
                if (i === 13) {
                    mesh.translateX(i * tileWidth - 7 * tileWidth + 0.2 * tileWidth);
                }
                else {
                    mesh.translateX(i * tileWidth - 7 * tileWidth);
                }
                mesh.rotation.x = Math.PI * 1.5;
                mesh.rotation.z = Math.PI * 1;
                scene.add(mesh);
                meshes.push(mesh);
            }
        }
    );
}

function printDebug(scene) {
    scene.traverse(function (obj) {
        var s = "|___";
        var obj2 = obj;
        while (obj2 !== scene) {
            s = "\t" + s;
            obj2 = obj2.parent;
        }
        console.log(s + obj.name + " <" + obj.type + ">");
    });
}

function resourceUrl(folder, filename) {
    return "/resources/" + folder + "/" + filename;
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
