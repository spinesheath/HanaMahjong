// Write your Javascript code.

function renderWwyd() {
    console.log("render wwyd");

    var container = document.querySelector('#container');

    var WIDTH = 400;
    var HEIGHT = 400;
    var VIEW_ANGLE = 45;
    var ASPECT = WIDTH / HEIGHT;
    var NEAR = 0.1;
    var FAR = 10000;

    var renderer = new THREE.WebGLRenderer();
    var camera = new THREE.PerspectiveCamera(VIEW_ANGLE, ASPECT, NEAR, FAR);
    var scene = new THREE.Scene();
    camera.position.z = 300;
    renderer.setSize(WIDTH, HEIGHT);
    container.append(renderer.domElement);

    //var loader = new THREE.TextureLoader();
    //var face = new THREE.MeshPhongMaterial({
    //    map: loader.load('test.png'),
    //    bumpMap: loader.load('bump.png')
    //});
    //var white = new THREE.MeshPhongMaterial({
    //    map: loader.load('white.png')
    //});

    var face = new THREE.MeshPhongMaterial({ color: 0xCC0000});
    var white = new THREE.MeshPhongMaterial({ color: 0xFFFFFF });

    var materials = [face, face, white, white, white, white];

    var geometry = new THREE.BoxGeometry(40, 60, 80);

    var mesh = new THREE.Mesh(geometry, materials);
    scene.add(mesh);

    var pointLight = new THREE.PointLight(0xFFFFFF);
    pointLight.position.x = 10;
    pointLight.position.y = 50;
    pointLight.position.z = 130;
    scene.add(pointLight);

    function update() {
        renderer.render(scene, camera);
        mesh.rotation.x += 0.01;
        mesh.rotation.y += 0.01;
        requestAnimationFrame(update);
    }
    requestAnimationFrame(update);
}
