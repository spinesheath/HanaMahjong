var replayContext;

const tileDepth = 0.78;
const tileWidth = 0.97;
const tileHeight = 1.28;
const gap = 0.2;

const _ids = {
    draw: 0,
    tsumogiri: 1,
    discardOffset: 2,
    agari: 50,
    ryuukyoku: 51,
    reach: 52,
    dora: 53,
    rinshan: 54,
    pon: 55,
    chii: 56,
    closedKan: 57,
    calledKan: 58,
    addedKan: 59
}

const allHandsOpen = true;
const showGhostTiles = false;

const announcements = {
    reach: { text: "reach" },
    pon: { text: "pon" },
    chii: { text: "chii" },
    kan: { text: "kan" },
    ron: { text: "ron" },
    tsumo: { text: "tsumo" },
    ryuukyoku: { text: "ryuukyoku" },
    material: new THREE.MeshBasicMaterial({ color: 0x777777 })
}

var replay;

function showFrame() {
    replayContext.createTiles(() => arrange());
}

function loadReplay() {
    const input = document.querySelector("#gameDataJson");
    const json = input.value;
    const data = JSON.parse(json);
    replay = parseReplay(data);
}

function initReplay() {
    replayContext = new RenderContext("replayCanvas");
    replayContext.setCameraPosition(0, -21, 31);
    replayCreateLights();
    replayContext.createTiles(() => arrange());
}

function arrange() {
    if (replay === undefined) {
        loadReplay();
    }

    const gameInput = document.querySelector("#gameId");
    const game = gameInput.value ? gameInput.value : 0;
    const frameInput = document.querySelector("#frameId");
    const frame = frameInput.value ? frameInput.value : 0;

    if (game < replay.length && game >= 0) {
        if (frame < replay[game].frames.length && frame >= 0) {
            arrangeFrame(replay[game].frames[frame]);
        }
    }
}

function arrangeFrame(frame) {
    createWall(frame);
    createHands(frame);
    createPonds(frame);
    createAnnouncements(frame);
}

function parseReplay(data) {
    const playerCount = 4;
    const defaultDoraIndicatorCount = 1;
    const akaDora = true;
   
    const games = [];
    let gameCount = data.games.length;
    for (let gameId = 0; gameId < gameCount; gameId++) {
        const game = {};
        game.frames = [];
        const setupFrame = {};
        setupFrame.id = 0;
        setupFrame.tilesDrawn = 13 * 4;
        setupFrame.rinshanTilesDrawn = 0;
        setupFrame.doraIndicators = defaultDoraIndicatorCount;
        setupFrame.static = { wall: data.games[gameId].wall, dice: data.games[gameId].dice, akaDora: akaDora, playerCount: playerCount, oya: data.games[gameId].oya };
        setStartingHands(setupFrame);
        setupFrame.ponds = [[], [], [], []];
        setupFrame.players = [{ reach: false }, { reach: false }, { reach: false }, { reach: false }];
        game.frames.push(setupFrame);

        let previousFrame = setupFrame;
        const decisions = data.games[gameId].actions;
        const decisionCount = decisions.length;
        for (let decisionId = 0; decisionId < decisionCount; decisionId++) {
            const decision = decisions[decisionId];

            if (decision === _ids.draw) {
                const frame = createDrawFrame(previousFrame);
                game.frames.push(frame);
                previousFrame = frame;
            } else if (decision === _ids.tsumogiri) {
                const frame = createTsumogiriFrame(previousFrame, decision);
                game.frames.push(frame);
                previousFrame = frame;
            } else if (decision > 1 && decision < 50) {
                const frame = createDiscardFrame(previousFrame, decision - _ids.discardOffset);
                game.frames.push(frame);
                previousFrame = frame;
            } else if (decision === _ids.pon) {
                const frames = createCallFrames(previousFrame, decisions.slice(decisionId + 1, decisionId + 4), announcements.pon);
                game.frames.push(frames[0], frames[1]);
                previousFrame = frames[1];
                decisionId += 3;
            } else if (decision === _ids.chii) {
                const frames = createCallFrames(previousFrame, decisions.slice(decisionId + 1, decisionId + 4), announcements.chii);
                game.frames.push(frames[0], frames[1]);
                previousFrame = frames[1];
                decisionId += 3;
            } else if (decision === _ids.calledKan) {
                const frames = createCallFrames(previousFrame, decisions.slice(decisionId + 1, decisionId + 5), announcements.kan);
                game.frames.push(frames[0], frames[1]);
                previousFrame = frames[1];
                decisionId += 4;
            } else if (decision === _ids.closedKan) {
                const frames = createClosedKanFrames(previousFrame, decisions.slice(decisionId + 1, decisionId + 5), announcements.kan);
                game.frames.push(frames[0], frames[1]);
                previousFrame = frames[1];
                decisionId += 4;
            } else if (decision === _ids.addedKan) {
                const frames = createAddedKanFrames(previousFrame, decisions.slice(decisionId + 1, decisionId + 5), announcements.kan);
                game.frames.push(frames[0], frames[1]);
                previousFrame = frames[1];
                decisionId += 4;
            } else if (decision === _ids.agari) {
                var who = decisions[decisionId + 1];
                var fromWho = decisions[decisionId + 2];
                const agariFrame = createAgariFrame(previousFrame, who, fromWho);
                game.frames.push(agariFrame);
                previousFrame = agariFrame;
                decisionId += 2;
            } else if (decision === _ids.ryuukyoku) {
                const frame = createRyuukyokuFrame(previousFrame);
                game.frames.push(frame);
                previousFrame = frame;
            } else if (decision === _ids.reach) {
                const reachFrame = createReachFrame(previousFrame);
                game.frames.push(reachFrame);
                previousFrame = reachFrame;
            } else if (decision === _ids.dora) {
                const frame = createDoraFrame(previousFrame);
                game.frames.push(frame);
                previousFrame = frame;
            } else if (decision === _ids.rinshan) { 
                const frame = createRinshanFrame(previousFrame);
                game.frames.push(frame);
                previousFrame = frame;
            }
        }

        games.push(game);
    }
    return games;
}

function createRyuukyokuFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.announcement = announcements.ryuukyoku;
    return frame;
}

function createAgariFrame(previousFrame, who, fromWho) {
    const activePlayer = who;

    const frame = cloneFrame(previousFrame);
    frame.players = frame.players.slice(0);
    frame.players[activePlayer] = Object.assign({}, frame.players[frame.activePlayer]);
    if (who === fromWho) {
        frame.players[activePlayer].announcement = announcements.tsumo;
    } else {
        frame.players[activePlayer].announcement = announcements.ron;
    }

    return frame;
}

function createRinshanFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.rinshanTilesDrawn += 1;

    const drawnTileId = frame.static.wall[frame.rinshanTilesDrawn];
    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);
    hand.tiles.push(drawnTileId);
    hand.drewRinshan = false;
    frame.hands[frame.activePlayer] = hand;
    return frame;
}

function createDoraFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.doraIndicators += 1;
    return frame;
}

function createAddedKanFrames(previousFrame, meldedTiles, announcement) {
    const activePlayer = previousFrame.activePlayer;

    const announcementFrame = cloneFrame(previousFrame);
    announcementFrame.players = announcementFrame.players.slice(0);
    announcementFrame.players[activePlayer] = Object.assign({}, announcementFrame.players[announcementFrame.activePlayer]);
    announcementFrame.players[activePlayer].announcement = announcement;

    const frame = cloneFrame(announcementFrame);

    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);

    removeMany(hand.tiles, meldedTiles);
    hand.justCalled = true;
    frame.hands[frame.activePlayer] = hand;

    hand.melds = hand.melds.slice(0);
    const pon = hand.melds.find(m => m.tiles.some(t => meldedTiles.indexOf(t) !== -1));
    const added = meldedTiles.find(t => pon.tiles.indexOf(t) === -1);
    hand.melds.push({ tiles: meldedTiles, flipped: pon.flipped, added: added, relativeFrom: pon.relativeFrom });
    remove(hand.melds, pon);

    return [announcementFrame, frame];
}

function createClosedKanFrames(previousFrame, meldedTiles, announcement) {
    const activePlayer = previousFrame.activePlayer;

    const announcementFrame = cloneFrame(previousFrame);
    announcementFrame.players = announcementFrame.players.slice(0);
    announcementFrame.players[activePlayer] = Object.assign({}, announcementFrame.players[announcementFrame.activePlayer]);
    announcementFrame.players[activePlayer].announcement = announcement;

    const frame = cloneFrame(announcementFrame);

    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);

    removeMany(hand.tiles, meldedTiles);
    hand.justCalled = true;
    frame.hands[frame.activePlayer] = hand;

    hand.melds = hand.melds.slice(0);
    hand.melds.push({ tiles: meldedTiles, relativeFrom: 0 });

    return [announcementFrame, frame];
}

function createCallFrames(previousFrame, meldedTiles, announcement) {
    const playerCalledFrom = previousFrame.activePlayer;
    const activePlayer = getCallingPlayerId(previousFrame, meldedTiles);

    const announcementFrame = cloneFrame(previousFrame);
    announcementFrame.activePlayer = activePlayer;
    announcementFrame.players = announcementFrame.players.slice(0);
    announcementFrame.players[activePlayer] = Object.assign({}, announcementFrame.players[activePlayer]);
    announcementFrame.players[activePlayer].announcement = announcement;

    const frame = cloneFrame(announcementFrame);

    frame.ponds = frame.ponds.slice(0);
    const pond = frame.ponds[playerCalledFrom].slice(0);
    const called = pond.pop();
    const ghostTile = Object.assign({}, called);
    pond.push(ghostTile);
    frame.ponds[playerCalledFrom] = pond;

    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[activePlayer]);
    hand.tiles = hand.tiles.slice(0);

    removeMany(hand.tiles, meldedTiles);
    hand.justCalled = true;
    frame.hands[activePlayer] = hand;

    hand.melds = hand.melds.slice(0);
    const relativeFrom = (playerCalledFrom - activePlayer + 4) % 4;
    const meld = { tiles: meldedTiles, flipped: called.tileId, relativeFrom: relativeFrom };
    hand.melds.push(meld);

    ghostTile.meld = meld;

    return [announcementFrame, frame];
}

function createReachFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.players = frame.players.slice(0);
    frame.players[frame.activePlayer] = Object.assign({}, frame.players[frame.activePlayer]);
    frame.players[frame.activePlayer].announcement = announcements.reach;
    frame.players[frame.activePlayer].reach = true;
    return frame;
}

function createTsumogiriFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);
    const tileId = hand.tiles.pop();
    sort(hand.tiles);
    hand.justCalled = false;
    hand.drewRinshan = false;
    frame.hands[frame.activePlayer] = hand;
    frame.ponds = frame.ponds.slice(0);
    const pond = frame.ponds[frame.activePlayer].slice(0);
    const flipped = frame.players[frame.activePlayer].reach && !pond.some(p => p.flipped && !p.meld);
    pond.push({ tileId: tileId, flipped: flipped });
    frame.ponds[frame.activePlayer] = pond;
    return frame;
}

function createDiscardFrame(previousFrame, indexInHand) {
    const frame = cloneFrame(previousFrame);
    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);
    const s = hand.tiles.slice(0);
    sort(s);
    const tileId = s[indexInHand];
    remove(hand.tiles, tileId);
    sort(hand.tiles);
    hand.justCalled = false;
    hand.drewRinshan = false;
    frame.hands[frame.activePlayer] = hand;
    frame.ponds = frame.ponds.slice(0);
    const pond = frame.ponds[frame.activePlayer].slice(0);
    const flipped = frame.players[frame.activePlayer].reach && !pond.some(p => p.flipped && !p.meld);
    pond.push({ tileId: tileId, flipped: flipped });
    frame.ponds[frame.activePlayer] = pond;
    return frame;
}

function createDrawFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.tilesDrawn += 1;
    if (frame.activePlayer === undefined) {
        frame.activePlayer = frame.static.oya;
    } else {
        frame.activePlayer = (frame.activePlayer + 1) % 4;
    }
    const drawnTileId = frame.static.wall[136 - frame.tilesDrawn];
    frame.hands = frame.hands.slice(0);
    const hand = Object.assign({}, frame.hands[frame.activePlayer]);
    hand.tiles = hand.tiles.slice(0);
    hand.tiles.push(drawnTileId);
    frame.hands[frame.activePlayer] = hand;
    return frame;
}

function cloneFrame(frame) {
    const clone = Object.assign({}, frame);
    clone.id += 1;
    if (clone.players.some(p => p.announcement)) {
        clone.players = frame.players.slice(0);
        const playerCount = frame.static.playerCount;
        for (let i = 0; i < playerCount; i++) {
            if (clone.players[i].announcement) {
                clone.players[i] = Object.assign({}, clone.players[i]);
                clone.players[i].announcement = undefined;
            }
        }
    }
    frame.announcement = undefined;
    return clone;
}

function getCallingPlayerId(frame, meldedTiles) {
    const playerCount = frame.static.playerCount;
    for (let i = 0; i < playerCount; i++) {
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

    const tileCount = frame.hands.length;
    for (let i = 0; i < tileCount; i++) {
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
        const meldCount = hand.melds.length;
        for (let meldId = 0; meldId < meldCount; meldId++) {
            const meld = hand.melds[meldId];
            meldX = createMeld(i, meldX, meld);
        }
    }
}

function createPonds(frame) {
    const playerCount = frame.static.playerCount;
    for (let playerId = 0; playerId < playerCount; playerId++) {
        const pond = frame.ponds[playerId].filter(p => showGhostTiles || p.meld === undefined);
        createPondRow(pond.slice(0, 6), 0, playerId);
        createPondRow(pond.slice(6, 12), 1, playerId);
        createPondRow(pond.slice(12), 2, playerId);
    }
}

function createPondRow(pondRow, row, playerId) {
    const a = -(3 * tileWidth);
    var x = a + 0.5 * tileWidth;
    const y = a - 0.5 * tileHeight - row * tileHeight;
    const tileCount = pondRow.length;
    for (let column = 0; column < tileCount; column++) {
        const pondTile = pondRow[column];
        const tileId = pondTile.tileId;
        const flip = pondTile.flipped ? 1 : 0;
        x += flip ? tileHeight - tileWidth : 0;
        if (pondTile.meld) {
            addGhostTile(playerId, tileId, x, y, 0, 0, flip);
        } else {
            addTile(playerId, tileId, x, y, 0, 0, flip);
        }
        x += tileWidth;
    }
}

function createAnnouncements(frame) {
    const playerCount = frame.static.playerCount;
    for (let playerId = 0; playerId < playerCount; playerId++) {
        createAnnouncement(frame.players[playerId].announcement);
    }
    createAnnouncement(frame.announcement);
}

function createAnnouncement(announcement) {
    if (!announcement) {
        return;
    }
    if (!announcement.mesh) {
        const text = announcement.text;
        const geometry = new THREE.TextBufferGeometry(text, { font: font, size: 2, height: 0, curveSegments: 2 });
        geometry.computeBoundingBox();
        const x = -0.5 * (geometry.boundingBox.max.x - geometry.boundingBox.min.x);
        const y = -0.5 * (geometry.boundingBox.max.y - geometry.boundingBox.min.y);
        const z = 2;
        const mesh = new THREE.Mesh(geometry, announcements.material);
        mesh.position.x = x;
        mesh.position.y = y;
        mesh.position.z = z;
        announcement.mesh = mesh;
    }

    replayContext.addMesh(announcement.mesh, false);
}

function createWall(frame) {
    const oyaId = frame.static.oya;
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
    const playerCount = frame.static.playerCount;
    for (let playerId = 0; playerId < playerCount; playerId++) {
        const tileIds = getDealtTileIds(frame.static.wall, playerId, frame.static.oya);
        frame.hands.push({ tiles: tileIds, melds: [], justCalled: false, drewRinshan: false });
    }
}

function getDealtTileIds(wall, playerId, oyaId) {
    var tileIds = [];
    const playerOffset = (playerId - oyaId  + 4) % 4;
    const offset = playerOffset * 4;
    for (let i = 0; i < 3; i++) {
        tileIds = tileIds.concat(wall.slice(136 - (offset + i * 16 + 4), 136 - (offset + i * 16)));
    }
    tileIds.push(wall[135 - (playerOffset + 3 * 4 * 4)]);
    sort(tileIds);
    return tileIds;
}

function createMeld(playerId, x, meld) {
    const b = -(11 * tileWidth);
    const y = b - 0.5 * tileHeight;
    const tileIds = meld.tiles.slice(0);
    tileIds.sort((a, b) => b - a);
    let isClosedKan = false;
    const tileCount = tileIds.length;
    if (meld.relativeFrom === 0) {
        isClosedKan = tileCount === 4;
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

    for (let i = 0; i < tileCount; i++) {
        const tileId = tileIds[i];
        const face = isClosedKan && (i === 0 || i === 3) ? 2 : 0;
        const isFlipped = tileId === meld.flipped;
        const flip = isFlipped ? 1 : 0;
        addTile(playerId, tileId, x, y, 0, face, flip);
        if (isFlipped && meld.added !== undefined) {
            addTile(playerId, tileId, x, y + tileWidth, 0, 0, 1);
        }
        x -= isFlipped ? tileHeight : tileWidth;
    }
    return x;
}

function addTile(playerId, tileId, x, y, z, open, flip) {
    const number = numberFromTileId(tileId);
    const suit = suitFromTileId(tileId);
    const mesh = replayContext.createTileMesh(number, suit);
    addTileMesh(mesh, playerId, x, y, z, open, flip);
}

function addGhostTile(playerId, tileId, x, y, z, open, flip) {
    const number = numberFromTileId(tileId);
    const suit = suitFromTileId(tileId);
    const mesh = replayContext.createGhostTileMesh(number, suit);
    addTileMesh(mesh, playerId, x, y, z, open, flip);
}

function addTileMesh(mesh, playerId, x, y, z, open, flip) {
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

    replayContext.addTile(mesh);
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
    const itemCount = values.length;
    for (let i = 0; i < itemCount; i++) {
        remove(array, values[i]);
    }
}

function sort(array) {
    array.sort((a, b) => a - b);
}
