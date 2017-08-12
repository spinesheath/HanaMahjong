var replayContext;

function initReplay() {
    replayContext = new RenderContext("replayCanvas");
    replayCreateLights();
}

function replayCreateLights() {
    var light = new THREE.AmbientLight(0x999999);
    replayContext.scene.add(light);
    var pointLight = new THREE.PointLight(0x555555);
    pointLight.position.x = 100;
    pointLight.position.y = 100;
    pointLight.position.z = 700;
    replayContext.scene.add(pointLight);
}
