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
