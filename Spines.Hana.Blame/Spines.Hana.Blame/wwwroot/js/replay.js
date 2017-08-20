﻿var replayContext;

const tileDepth = 0.78;
const tileWidth = 0.97;
const tileHeight = 1.28;
const gap = 0.2;

const initId = 400;
const agariId = 300;
const addedKanId = 200;
const calledKanId = 201;
const closedKanId = 202;
const ponId = 202;
const chiiId = 202;

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

    const games = parseReplay(data);

    arrangeFrame(games[0].frames[2]);
}

function arrangeFrame(frame) {
    createWall(frame);
    createHands(frame);
    createPonds(frame);
}

function parseReplay(data) {
    const playerCount = 4;
    const defaultDoraIndicatorCount = 1;
    const akaDora = true;

    const rawData = splitRawData(data);

    const games = [];
    for (let gameId = 0; gameId < rawData.length; gameId++) {
        const game = {};
        game.frames = [];
        const frame0 = {};
        frame0.id = 0;
        frame0.oya = 0;
        frame0.tilesDrawn = 13 * 4;
        frame0.rinshanTilesDrawn = 0;
        frame0.doraIndicators = defaultDoraIndicatorCount;
        frame0.static = { wall: rawData[gameId].wall, dice: rawData[gameId].dice, akaDora: akaDora, playerCount: playerCount };
        setStartingHands(frame0);
        frame0.ponds = [[], [], [], []];
        game.frames.push(frame0);

        var previousFrame = frame0;
        const decisions = rawData[gameId].decisions;
        let activePlayer = 0;
        for (let decisionId = 0; decisionId < decisions.length; decisionId++) {
            const drawFrame = createDrawFrame(previousFrame, activePlayer);
            game.frames.push(drawFrame);
            previousFrame = drawFrame;

            const decision = decisions[decisionId];
            // discard
            if (decision < 136) {
                const discardFrame = createDiscardFrame(previousFrame, activePlayer, decision);
                game.frames.push(discardFrame);
                previousFrame = discardFrame;
            } else if (decision === agariId) {
                
            } else if (decision === ponId) {
                
            } else if (decision === chiiId) {

            } else if (decision === calledKanId) {

            } else if (decision === closedKanId) {

            } else if (decision === addedKanId) {

            }
            break;
        }

        games.push(game);
    }
    return games;
}

function createDiscardFrame(previousFrame, activePlayer, tileId) {
    const discardFrame = Object.assign({}, previousFrame);
    discardFrame.id += 1;
    discardFrame.hands = discardFrame.hands.slice(0);
    const tileIds = discardFrame.hands[activePlayer].slice(0);
    var index = tileIds.indexOf(tileId);
    tileIds.splice(index, 1);
    tileIds.sort((a, b) => a - b);
    discardFrame.hands[activePlayer] = tileIds;
    discardFrame.ponds = discardFrame.ponds.slice(0);
    const pond = discardFrame.ponds[activePlayer].slice(0);
    pond.push(tileId);
    discardFrame.ponds[activePlayer] = pond;
    return discardFrame;
}

function createDrawFrame(previousFrame, activePlayer) {
    const drawFrame = Object.assign({}, previousFrame);
    drawFrame.id += 1;
    drawFrame.tilesDrawn += 1;
    const drawnTileId = drawFrame.static.wall[136 - drawFrame.tilesDrawn];
    drawFrame.hands = drawFrame.hands.slice(0);
    const tileIds = drawFrame.hands[activePlayer].slice(0);
    tileIds.push(drawnTileId);
    drawFrame.hands[activePlayer] = tileIds;
    return drawFrame;
}

function createHands(frame) {
    const a = -(14 * tileWidth + gap) / 2;
    const b = -(11 * tileWidth);
    const y = b - 0.5 * tileHeight;
    const startX = a + 0.5 * tileWidth - tileWidth;

    for (let i = 0; i < frame.hands.length; i++) {
        let x = startX;
        const handTiles = frame.hands[i];
        const tilesInHand = handTiles.length;
        for (let k = 0; k < tilesInHand; k++) {
            const tileId = handTiles[k];
            addTile(i, tileId, x, y, 0, i === 0 ? -0.40 : 3, 0);
            if (k === tilesInHand - 2 && tilesInHand % 3 === 2) {
                x += gap;
            }
            x += tileWidth;
        }
    }
}

function createPonds(frame) {
    const a = -(3 * tileWidth);
    const x = a + 0.5 * tileWidth;
    const y = a - 0.5 * tileHeight;
    for (let playerId = 0; playerId < frame.static.playerCount; playerId++) {
        const pond = frame.ponds[playerId];
        for (let i = 0; i < pond.length; i++) {
            const row = Math.min(Math.floor(i / 6), 3);
            const column = i - 6 * row;
            const tileId = pond[i];
            addTile(playerId, tileId, x + column * tileWidth, y - row * tileHeight, 0, 0, 0);
        }
    }
}

function createWall(frame) {
    const oyaId = frame.oya;
    const dice = frame.static.dice;
    const wall = frame.static.wall;
    const tilesDrawn = frame.tilesDrawn;
    const rinshanTilesDrawn = frame.rinshanTilesDrawn;
    const doraIndicators = frame.doraIndicators;

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
                    const isDoraIndicator = shiftedId > 4 && shiftedId % 2 === 1 && shiftedId < 4 + doraIndicators * 2;
                    addTile(i, tileId, x, y, k * tileDepth, isDoraIndicator ? 0 : 2, 0);
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

function setStartingHands(frame) {
    frame.hands = [];
    for (let playerId = 0; playerId < frame.static.playerCount; playerId++) {
        const tileIds = getDealtTileIds(frame.static.wall, playerId, frame.oya);
        frame.hands.push(tileIds);
    }
}

function splitRawData(data) {
    const tileCount = 136;
    const diceCount = 2;

    const rawData = [];
    var i = 0;
    while (i < data.length && data[i] !== initId) {
        i += 1;
    }
    while (i < data.length) {
        i += 1;
        const wall = data.slice(i, i + tileCount);
        i += tileCount;
        const dice = data.slice(i, i + diceCount);
        i += diceCount;
        let c = 0;
        while (data[i + c] !== initId && i + c < data.length) {
            c += 1;
        }
        const decisions = data.slice(i, i + c);
        rawData.push({ wall: wall, dice: dice, decisions: decisions });
        i += c;
    }
    return rawData;
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
