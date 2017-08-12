var replayScene;
var replayRenderer;
var replayCamera;
var replayGeometry;

function initReplay() {

    window.onpopstate = function (e) {

    };

    var canvas = document.querySelector("#replayCanvas");
    var displayWidth = canvas.clientWidth;
    var displayHeight = canvas.clientHeight;
    replayRenderer = new THREE.WebGLRenderer({ canvas: canvas, antialias: true });
    replayRenderer.setSize(displayWidth, displayHeight);

    replayCamera = createCamera(displayWidth, displayHeight);
    replayScene = new THREE.Scene();

    THREE.DefaultLoadingManager.onLoad = replayRender;

    replayCreateLights(replayScene);
}

function replayRender() {
    replayRenderer.render(replayScene, replayCamera);
}

function replayCreateCamera(width, height) {
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

function replayCreateLights(scene) {
    var light = new THREE.AmbientLight(0x999999);
    scene.add(light);
    var pointLight = new THREE.PointLight(0x555555);
    pointLight.position.x = 100;
    pointLight.position.y = 100;
    pointLight.position.z = 700;
    scene.add(pointLight);
}
