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
const ponId = 203;
const chiiId = 204;

const allHandsOpen = true;

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

    arrangeFrame(games[0].frames[3]);
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
        const setupFrame = {};
        setupFrame.id = 0;
        setupFrame.oya = 0;
        setupFrame.tilesDrawn = 13 * 4;
        setupFrame.rinshanTilesDrawn = 0;
        setupFrame.doraIndicators = defaultDoraIndicatorCount;
        setupFrame.activePlayer = 0;
        setupFrame.static = { wall: rawData[gameId].wall, dice: rawData[gameId].dice, akaDora: akaDora, playerCount: playerCount };
        setStartingHands(setupFrame);
        setupFrame.ponds = [[], [], [], []];
        game.frames.push(setupFrame);

        let previousFrame = setupFrame;
        const decisions = rawData[gameId].decisions;
        for (let decisionId = 0; decisionId < decisions.length; decisionId++) {
            const decision = decisions[decisionId];

            // discard
            if (decision < 136) {
                const frame = createDrawFrame(previousFrame);
                game.frames.push(frame);
                previousFrame = frame;
                const discardFrame = createDiscardFrame(previousFrame, decision);
                game.frames.push(discardFrame);
                previousFrame = discardFrame;
            } else if (decision === ponId) {
                const frame = createCallFrame(previousFrame, decisions.slice(decisionId + 1, decisionId + 4));
                game.frames.push(frame);
                previousFrame = frame;
                decisionId += 3;
                break;
            } else if (decision === chiiId) {
                const frame = createCallFrame(previousFrame, decisions.slice(decisionId + 1, decisionId + 4));
                game.frames.push(frame);
                previousFrame = frame;
                decisionId += 3;
                break;
            } else if (decision === calledKanId) {
                const frame = createCallFrame(previousFrame, decisions.slice(decisionId + 1, decisionId + 5));
                game.frames.push(frame);
                previousFrame = frame;
                decisionId += 4;
                break;
            } else if (decision === closedKanId) {
                decisionId += 4;
            } else if (decision === addedKanId) {
                decisionId += 4;
            } else if (decision === agariId) {
                decisionId += 2;
            }
        }

        games.push(game);
    }
    return games;
}

function createCallFrame(previousFrame, meldedTiles) {
    const frame = Object.assign({}, previousFrame);
    const activePlayer = getCallingPlayerId(frame, meldedTiles);
    frame.ponds = frame.ponds.slice(0);
    const pond = frame.ponds[previousFrame.activePlayer].slice(0);
    const called = pond.pop();
    frame.ponds[previousFrame.activePlayer] = pond;
    frame.activePlayer = activePlayer;
    frame.id += 1;
    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);
    removeMany(hand.tiles, meldedTiles);
    hand.justCalled = true;
    frame.hands[frame.activePlayer] = hand;

    hand.melds = hand.melds.slice(0);
    const relativeFrom = (previousFrame.activePlayer - activePlayer + 4) % 4;
    hand.melds.push({tiles: meldedTiles, flipped: called, relativeFrom: relativeFrom});

    return frame;
}

function createDiscardFrame(previousFrame, tileId) {
    const frame = Object.assign({}, previousFrame);
    frame.id += 1;
    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);
    remove(hand.tiles, tileId);
    sort(hand.tiles);
    hand.justCalled = false;
    frame.hands[frame.activePlayer] = hand;
    frame.ponds = frame.ponds.slice(0);
    const pond = frame.ponds[frame.activePlayer].slice(0);
    pond.push(tileId);
    frame.ponds[frame.activePlayer] = pond;
    return frame;
}

function createDrawFrame(previousFrame) {
    const frame = Object.assign({}, previousFrame);
    frame.id += 1;
    frame.tilesDrawn += 1;
    const drawnTileId = frame.static.wall[136 - frame.tilesDrawn];
    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);
    hand.tiles.push(drawnTileId);
    hand.justCalled = false;
    frame.hands[frame.activePlayer] = hand;
    return frame;
}

function getCallingPlayerId(frame, meldedTiles) {
    for (let i = 0; i < frame.static.playerCount; i++) {
        if (meldedTiles.some(x => frame.hands[i].tiles.indexOf(x) !== -1)) {
            return i;
        }
    }
    throw "no player found for call";
}

function createHands(frame) {
    const a = -(14 * tileWidth + gap) / 2;
    const b = -(11 * tileWidth);
    const y = b - 0.5 * tileHeight;
    const handStartX = a + 0.5 * tileWidth - tileWidth;
    const meldStartX = -a + 4 * tileWidth;
    const flip = (allHandsOpen ? 0 : 3);

    for (let i = 0; i < frame.hands.length; i++) {
        let x = handStartX;
        const hand = frame.hands[i];
        const tilesInHand = hand.tiles.length;
        for (let k = 0; k < tilesInHand; k++) {
            const tileId = hand.tiles[k];
            addTile(i, tileId, x, y, 0, i === 0 ? -0.40 : flip, 0);
            if (!frame.hands[i].justCalled && k === tilesInHand - 2 && tilesInHand % 3 === 2) {
                x += gap;
            }
            x += tileWidth;
        }

        let meldX = meldStartX;
        for (let meldId = 0; meldId < hand.melds.length; meldId++) {
            const meld = hand.melds[meldId];
            meldX = createMeld(i, meldX, meld);
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
        frame.hands.push({ tiles: tileIds, melds: [], justCalled: false });
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
    tileIds.push(wall[135 - ((playerId - oyaId) + 3 * 4 * 4)]);
    sort(tileIds);
    return tileIds;
}

function createMeld(playerId, x, meld) {
    const b = -(11 * tileWidth);
    const y = b - 0.5 * tileHeight;
    const tileIds = meld.tiles.slice(0);
    tileIds.sort((a, b) => b - a);
    let isClosedKan = false;
    if (meld.relativeFrom === 0) {
        isClosedKan = tileIds.length === 4;
    } else {
        remove(tileIds, meld.flipped);
        remove(tileIds, meld.added);
        if (meld.relativeFrom === 1) {
            tileIds.splice(0, 0, meld.flipped);
        } else if (meld.relativeFrom === 2) {
            tileIds.splice(1, 0, meld.flipped);
        } else if (meld.relativeFrom === 3) {
            tileIds.push(meld.flipped);
        }
    }
    
    for (let i = 0; i < tileIds.length; i++) {
        const tileId = tileIds[i];
        const face = isClosedKan && (i === 0 || i === 3) ? 2 : 0;
        const isFlipped = tileId === meld.flipped;
        const flip = isFlipped ? 1 : 0;
        addTile(playerId, tileId, x, y, 0, face, flip);
        if (isFlipped && meld.added !== undefined) {
            addTile(playerId, tileId, x, y + tileWidth, 0, 2, 1);
        }
        x -= isFlipped ? tileHeight : tileWidth;
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

function remove(array, value) {
    const index = array.indexOf(value);
    if (index >= 0) {
        array.splice(index, 1);
    }
}

function removeMany(array, values) {
    for (let i = 0; i < values.length; i++) {
        remove(array, values[i]);
    }
}

function sort(array) {
    array.sort((a, b) => a - b);
}
