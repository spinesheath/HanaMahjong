var replayContext;

function initReplay() {
    replayContext = new RenderContext("replayCanvas");
    replayContext.setCameraPosition(0, 0, 25);
    replayCreateLights();

    replayContext.createTiles(() => arrange());
}

const tileDepth = 0.78;
const tileWidth = 0.97;
const tileHeight = 1.3;

function arrange() {
    createWall();
    createPond();
}

function createPond() {
    const a = -(3 * tileWidth);
    var tileId = 0;
    for (let i = 0; i < 4; i++) {
        let x = a + 0.5 * tileWidth;
        let y = a - 0.5 * tileHeight;
        for (let j = 0; j < 3; j++) {
            for (let k = 0; k < 6; k++) {
                addTile(i, tileId, x, y, 0, true);
                tileId += 1;
                x += tileWidth;
            }
            y -= tileHeight;
            x = a + 0.5 * tileWidth;
        }
    }
}

function createWall() {
    const gap = 0.2;
    const a = -(17 * tileWidth + tileHeight + gap) / 2;
    var tileId = 0;
    for (let i = 0; i < 4; i++) {
        let x = a + 0.5 * tileWidth + tileHeight + gap;
        const y = a + 0.5 * tileHeight;
        for (let j = 0; j < 17; j++) {
            for (let k = 0; k < 2; k++) {
                addTile(i, tileId, x, y, k * tileDepth, tileId === 21);
                tileId += 1;
            }
            if (j === 7 && i === 0) {
                x += gap;
            }
            if (j === 14 && i === 0) {
                x += gap;
            }
            x += tileWidth;
        }
    }
}

function addTile(playerId, tileId, x, y, z, open) {
    const number = numberFromTileId(tileId);
    const suit = suitFromTileId(tileId);
    const mesh = replayContext.createTileMesh(number, suit);

    mesh.rotateZ(Math.PI * 0.5 * playerId);
    mesh.translateX(x);
    mesh.translateY(y);
    mesh.translateZ(z);
    mesh.rotateX(Math.PI * 0.5);
    mesh.rotateY(Math.PI);
    if (!open) {
        mesh.rotateZ(Math.PI);
    }

    replayContext.scene.add(mesh);
}

function suitFromTileId(tileId) {
    return Math.floor(tileId / (9 * 4));
}

function numberFromTileId(tileId) {
    if (tileId === 4 * 4 || tileId === 16 + 9 * 4 || tileId === 16 + 9 * 4 * 2)
        return 0;
    return 1 + Math.floor((tileId % (9 * 4)) / 4);
}

function replayCreateLights() {
    const light = new THREE.AmbientLight(0x999999);
    replayContext.scene.add(light);
    const pointLight = new THREE.PointLight(0x555555);
    pointLight.position.x = 100;
    pointLight.position.y = 100;
    pointLight.position.z = 700;
    replayContext.scene.add(pointLight);
}
