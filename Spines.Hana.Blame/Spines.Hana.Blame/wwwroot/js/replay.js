var replayContext;
const tileDepth = 0.78;
const tileWidth = 0.97;
const tileHeight = 1.28;
const gap = 0.2;

function initReplay() {
    replayContext = new RenderContext("replayCanvas");
    replayContext.setCameraPosition(0, -21, 31);
    replayCreateLights();

    replayContext.createTiles(() => arrange());
}

function arrange() {
    const input = document.querySelector("#gameDataJson");
    const json = input.value;
    const data = JSON.parse(json);
    const wall = data.slice(0, 136);
    const dice = data.slice(136, 138);

    const oyaId = 0;
    const tilesDrawn = 13 * 4;
    const rinshanTilesDrawn = 0;

    createWall(wall, dice, oyaId, tilesDrawn, rinshanTilesDrawn);
    //createPond();
    createHands(wall, oyaId);
}

function createHands(wall, oyaId) {
    const a = -(14 * tileWidth + gap) / 2;
    const b = -(11 * tileWidth);
    const y = b - 0.5 * tileHeight;
    for (let i = 0; i < 4; i++) {
        let x = a + 0.5 * tileWidth - tileWidth;
        const handTiles = getDealtTileIds(wall, i, oyaId);
        const tilesInHand = 13;
        for (let k = 0; k < handTiles.length; k++) {
            const tileId = handTiles[k];
            addTile(i, tileId, x, y, 0, i === 0 ? -0.40 : 3, 0);
            if (k === tilesInHand - 2 && tilesInHand % 3 === 2) {
                x += gap;
            }
            x += tileWidth;
        }

        //let meldX = -a + 4 * tileWidth;
        //const meldCount = Math.floor((14 - tilesInHand) / 3);
        //for (let j = 0; j < meldCount; ++j) {
        //    const meldTileIds = [wall[135 - wallId++], wall[135 - wallId++], wall[135 - wallId++], wall[135 - wallId++]];
        //    meldX = createMeld(i, meldX, meldTileIds);
        //}
    }
}

function getDealtTileIds(wall, playerId, oyaId) {
    var tileIds = [];
    const offset = (playerId - oyaId) * 4;
    for (let i = 0; i < 3; i++) {
        tileIds = tileIds.concat(wall.slice(136 - (offset + i * 16 + 4), 136 - (offset + i * 16)));
    }
    tileIds.push(wall[135 - (offset + 3 * 4 * 4)]);
    tileIds.sort((a, b) => a - b);
    return tileIds;
}

function createWall(wall, dice, oyaId, tilesDrawn, rinshanTilesDrawn) {
    const layoutOffset = -(17 * tileWidth + tileHeight + gap) / 2;
    const y = layoutOffset + 0.5 * tileHeight;
    const xStart = layoutOffset + 0.5 * tileWidth + tileHeight + gap;

    const diceSum = dice[0] + dice[1];
    const wallOffset = (20 - diceSum - oyaId) * 34 + diceSum * 2;

    var wallId = 0;

    for (let i = 0; i < 4; i++) {
        let x = xStart;
        for (let j = 0; j < 17; j++) {
            for (let k = 0; k < 2; k++) {
                const shiftedId = (wallId + wallOffset) % 136;
                if (shiftedId === 0) {
                    x += gap;
                }
                const rinshanId = shiftedId % 2 === 0 ? shiftedId + 1 : shiftedId - 1;
                if (rinshanId >= rinshanTilesDrawn && shiftedId < 136 - tilesDrawn) {
                    const tileId = wall[shiftedId];
                    addTile(i, tileId, x, y, k * tileDepth, shiftedId === 5 ? 0 : 2, 0);
                }
                wallId += 1;
                if (shiftedId === 13) {
                    x += gap;
                }
            }
            x += tileWidth;
        }
    }
}

function createMeld(playerId, x, tileIds) {
    const b = -(11 * tileWidth);
    const y = b - 0.5 * tileHeight;
    for (let i = 0; i < tileIds.length; i++) {
        const tileId = tileIds[i];
        if (tileIds.length === 4 && i === 0) {
            addTile(playerId, tileId, x, y, 0, 2, 0);
            x -= tileWidth;
        } else if (tileIds.length === 4 && i === 3) {
            addTile(playerId, tileId, x, y, 0, 0, 1);
            x -= tileHeight;
        } else {
            addTile(playerId, tileId, x, y, 0, 0, 0);
            x -= tileWidth;
        }
    }
    return x;
}

function createPond() {
    const a = -(3 * tileWidth);
    var tileId = 0;
    for (let i = 0; i < 4; i++) {
        let x = a + 0.5 * tileWidth;
        let y = a - 0.5 * tileHeight;
        for (let j = 0; j < 3; j++) {
            for (let k = 0; k < 6; k++) {
                addTile(i, tileId, x, y, 0, 0, 0);
                tileId += 1;
                x += tileWidth;
            }
            y -= tileHeight;
            x = a + 0.5 * tileWidth;
        }
    }
}

function addTile(playerId, tileId, x, y, z, open, flip) {
    const number = numberFromTileId(tileId);
    const suit = suitFromTileId(tileId);
    const mesh = replayContext.createTileMesh(number, suit);

    if (flip % 2 === 1) {
        const a = (tileHeight - tileWidth) / 2;
        x -= a;
        y -= a;
    }

    mesh.rotateZ(Math.PI * 0.5 * playerId);
    mesh.translateX(x);
    mesh.translateY(y);
    mesh.translateZ(z);
    mesh.rotateY(Math.PI);
    mesh.rotateX(Math.PI * 0.5 * (open - 1));
    mesh.rotateY(Math.PI * 0.5 * (flip));

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
