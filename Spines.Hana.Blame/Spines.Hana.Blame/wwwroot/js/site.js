var scene;
var renderer;
var camera;
var material;
var geometry;

function changeUrl(val) {
    createTiles(parseWwyd(val));
    window.history.pushState(val, "", "//" + location.host + location.pathname + "?h=" + val);
}

function initWwyd() {

    window.onpopstate = function (e) {
        if (e.state) {
            var handInput = document.getElementById("handInput");
            handInput.value = e.state;
            createTiles(parseWwyd(handInput.value));
        }
    };

    var displayHeight = 100;
    var displayWidth = 600;

    var canvas = document.querySelector("#wwydCanvas");
    renderer = new THREE.WebGLRenderer({ canvas: canvas, antialias: true });
    renderer.setSize(displayWidth, displayHeight);

    camera = createCamera(displayWidth, displayHeight);
    scene = new THREE.Scene();

    THREE.DefaultLoadingManager.onLoad = render;

    createLights(scene);
    createTiles(parseWwyd(document.getElementById("handInput").value));
}

function createTiles(tileTypes) {
    removeMeshes();
    createMaterial();

    if (geometry === undefined) {
        var jsonloader = new THREE.JSONLoader();
        jsonloader.load(resourceUrl("geometries", "tile.json"),
            function(g) {
                geometry = g;
                arrangeTiles(tileTypes);
            }
        );
    } else {
        arrangeTiles(tileTypes);
        render();
    }
}

function arrangeTiles(tileTypes) {
    for (var i = 0; i < tileTypes.length && i < 14; i++) {
        var tileType = tileTypes[i];
        var x = tileType[0]; // number
        var y = tileType[1]; // suit
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
    }
}

function printDebug() {
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

function render() {
    renderer.render(scene, camera);
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

function removeMeshes() {
    for (var k = scene.children.length - 1; k >= 0; k--) {
        var child = scene.children[k];
        if (child.type === "Mesh") {
            scene.remove(child);
        }
    }
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

// returns an array of arrays [number, suit] ints
function parseWwyd(hand) {
    try {
        var suits = "mpsz";
        var numbers = "0123456789";
        if (hand == undefined || hand.length > 50) {
            return [];
        }
        // var counts = new int[suits.Length * numbers.Length];
        var counts = new Array(suits.length * numbers.length);
        counts.fill(0);
        var reverse = hand.split("").reverse();
        var offset = -1;

        for (var i = 0; i < reverse.length; i++) {
            var c = reverse[i];
            var suit = suits.indexOf(c);
            if (suit >= 0) {
                offset = 10 * suit;
                continue;
            }
            if (offset < 0) {
                return [];
            }
            var number = numbers.indexOf(c);
            if (number >= 0) {
                counts[offset + number] += 1;
                continue;
            }
            return [];
        }

        // no red 5 of 8 or 9 for honors.
        if (counts[30] + counts[38] + counts[39] > 0) {
            return [];
        }
        // at most 4 of each tile.
        if (counts.some(b => b > 4)) {
            return [];
        }
        // at most 1 red 5 each suit.
        if (counts[0] > 1 || counts[10] > 1 || counts[20] > 1) {
            return [];
        }
        // with a red 5, only 3 other 5 at most.
        if ((counts[0] > 0 || counts[10] > 0 || counts[20] > 0) && (counts[5] > 3 || counts[15] > 3 || counts[25] > 3)) {
            return [];
        }
        var sum = counts.reduce((a, b) => a + b, 0);
        if (sum < 13 || sum > 14) {
            return [];
        }

        var result = [];
        for (var j = 0; j < counts.length; j++) {
            for (var k = 0; k < counts[j]; k++) {
                var n = j % 10;
                var s = (j - n) / 10;
                result.push([n, s]);
            }
        }
        return result;
    }
    catch (e) {
        return [];
    }
}
