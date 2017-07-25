function changeUrl(val) {
    document.location = "//" + location.host + location.pathname + "?h=" + val;
}

function renderWwyd() {
    var displayHeight = 100;
    var displayWidth = 400;
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
    createLights(scene);
    createTiles(scene, tileTypes);
    
    function update() {
        renderer.render(scene, camera);
        requestAnimationFrame(update);
    }
    requestAnimationFrame(update);
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

function createTiles(scene, tileTypes) {
    //  27mm : 20mm : 16mm
    var geometry = new THREE.BoxGeometry(48, 64, 38);

    var faceUvs = makeUv(24 / 32, 1);
    geometry.faceVertexUvs[0][8] = [faceUvs[0], faceUvs[1], faceUvs[3]];
    geometry.faceVertexUvs[0][9] = [faceUvs[1], faceUvs[2], faceUvs[3]];
    geometry.faceVertexUvs[0][10] = [faceUvs[0], faceUvs[1], faceUvs[3]];
    geometry.faceVertexUvs[0][11] = [faceUvs[1], faceUvs[2], faceUvs[3]];

    var rightUvs = makeUv(19 / 32, 1);
    geometry.faceVertexUvs[0][0] = [rightUvs[0], rightUvs[1], rightUvs[3]];
    geometry.faceVertexUvs[0][1] = [rightUvs[1], rightUvs[2], rightUvs[3]];
    geometry.faceVertexUvs[0][2] = [rightUvs[2], rightUvs[3], rightUvs[1]];
    geometry.faceVertexUvs[0][3] = [rightUvs[3], rightUvs[0], rightUvs[1]];

    var topUvs = makeUv(24 / 32, 19 / 32);
    geometry.faceVertexUvs[0][4] = [topUvs[0], topUvs[1], topUvs[3]];
    geometry.faceVertexUvs[0][5] = [topUvs[1], topUvs[2], topUvs[3]];
    geometry.faceVertexUvs[0][6] = [topUvs[2], topUvs[3], topUvs[1]];
    geometry.faceVertexUvs[0][7] = [topUvs[3], topUvs[0], topUvs[1]];

    var loader = new THREE.TextureLoader();
    var back = new THREE.MeshPhongMaterial({ map: loader.load(textureUrl("other", "back.png")) });
    var side = new THREE.MeshPhongMaterial({ map: loader.load(textureUrl("other", "side.png")) });
    var top = new THREE.MeshPhongMaterial({ map: loader.load(textureUrl("other", "top.png")) });

    for (i = 0; i < tileTypes.length && i < 14; i++) {
        var face = loadFace(loader, tileTypes[i]);

        // right, left, top, bottom, front, back
        var materials = [side, side, top, top, face, back];
        var mesh = new THREE.Mesh(geometry, materials);
        scene.add(mesh);
        if (i === 13) {
            mesh.translateX(i * 48 - 7 * 48 + 10);
        }
        else {
            mesh.translateX(i * 48 - 7 * 48);
        }
    }
}

function textureUrl(folder, filename) {
    return "/textures/" + folder + "/" + filename;
}

function createCamera(width, height) {
    var HEIGHT = 400;
    var VIEW_ANGLE = 45;
    var NEAR = 0.1;
    var FAR = 10000;
    var tanFOV = Math.tan(((Math.PI / 180) * VIEW_ANGLE / 2));
    var fov = (360 / Math.PI) * Math.atan(tanFOV * (height / HEIGHT));
    var cameraAspect = width / height;

    var camera = new THREE.PerspectiveCamera(fov, cameraAspect, NEAR, FAR);
    camera.position.x = 0;
    camera.position.y = 0;
    camera.position.z = 1000;
    camera.lookAt(new THREE.Vector3(0, 0, 0));
    return camera;
}

function loadFace(loader, tileType) {
    var face = new THREE.MeshPhongMaterial({
        map: loader.load(textureUrl("face", tileType + ".png")),
        bumpMap: loader.load(textureUrl("bump", tileType + ".png"))
    });
    return face;
}

function makeUv(width, height) {
    return [
        new THREE.Vector2(0, height),
        new THREE.Vector2(0, 0),
        new THREE.Vector2(width, 0),
        new THREE.Vector2(width, height)
    ];
}


