var replayContext;
var _observedPlayerId;
var _replay;
var _replayJson;

function updateHistory(d) {
    const c = window.history.state;
    if (!c || c.r !== d.r || c.p !== d.p || c.g !== d.g || c.f !== d.f)
        setBrowserHistory(d);
}

function onReplayIdChanged(replayId) {
    const d = { r: replayId, p: 0, g: 0, f: 0 };
    loadReplay(d);
}

function loadReplay(d) {
    setValueToInput("#replayId", d.r);
    if (!d.r) {
        _replayJson = undefined;
        setFrameInputData(d);
        updateHistory(d);
        replayContext.createTiles(() => arrange());
        return;
    }

    const xhr = $.ajax({
        type: "GET",
        url: "/Home/Replay",
        data: { replayId: d.r },
        success: function (data, textStatus, xhr2) {
            const r = getReplayId();
            if (xhr2.replayId === r) {
                _replayJson = data;
                setFrameInputData(d);
                updateHistory(d);
                replayContext.createTiles(() => arrange());
            }
        }
    });
    xhr.replayId = d.r;
}

function getReplayId() {
    return getValueFromComboBox("#replayId");
}

function getFrameInputDataFromUrl() {
    const params = new URLSearchParams(window.location.search);
    const replayId = getStringFromParams(params, "r");
    const playerId = getIntFromParams(params, "p");
    const game = getIntFromParams(params, "g");
    const frame = getIntFromParams(params, "f");
    return { r: replayId, p: playerId, g: game, f: frame };
}

function getFrameInputData() {
    const replayId = getReplayId();
    const playerId = getIntFromInput("#playerId");
    const game = getIntFromInput("#gameId");
    const frame = getIntFromInput("#frameId");
    return { r: replayId, p: playerId, g: game, f: frame };
}

function setFrameInputData(data) {
    setValueToInput("#playerId", data.p);
    setValueToInput("#gameId", data.g);
    setValueToInput("#frameId", data.f);
}

function initReplay() {
    window.onpopstate = onPopState;

    replayContext = new RenderContext("replayCanvas");
    replayContext.setCameraPosition(_cameraPosition, _lookAt);
    replayContext.createAmbientLight(_ambientLightColor);
    replayContext.createPointLight(_pointLightColor, _pointLightPosition);
    
    const d = getFrameInputDataFromUrl();
    loadReplay(d);
}

function onPopState(e) {
    const d = e.state;
    if (d) {
        setFrameInputData(d);
        replayContext.createTiles(() => arrange());
    }
}

function onFrameChanged() {
    const d = getFrameInputData();
    updateHistory(d);
    replayContext.createTiles(() => arrange());
}

function getIntFromParams(params, key) {
    return parseInt(params.get(key)) || 0;
}

function getStringFromParams(params, key) {
    if (!params.has(key))
        return undefined;
    return params.get(key);
}

function arrange() {
    if (!_replayJson) {
        return;
    }

    const d = getFrameInputDataFromUrl();
    const playerId = d.p;
    const game = d.g;
    const frame = d.f;

    if (playerId < 0 || playerId > 3) {
        return;
    }
    _observedPlayerId = playerId;

    if (_replay === undefined) {
        _replay = parseReplay(_replayJson);
    }
    
    if (game < _replay.length && game >= 0) {
        if (frame < _replay[game].frames.length && frame >= 0) {
            arrangeFrame(_replay[game].frames[frame]);
        }
    }
}

function arrangeFrame(frame) {
    createWall(frame);
    createHands(frame);
    createPonds(frame);
    createAnnouncements(frame);
    createBa(frame);
    createPlayerInfos(frame);
}

function createPlayerInfos(frame) {
    const staticPlayers = frame.static.players;
    const players = frame.players;
    const playerCount = frame.static.playerCount;
    for (let playerId = 0; playerId < playerCount; playerId++) {
        createPlayerInfo(staticPlayers[playerId], players[playerId], playerId);
    }
}

function createPlayerInfo(staticPlayer, player, playerId) {
    const p = getRotatedPlayerId(playerId);
    const div = document.querySelector(`#playerInfo${p}`);
    div.textContent =
        staticPlayer.name + "\r\n" +
        staticPlayer.gender + " " + _ranks[staticPlayer.rank] + " R" + staticPlayer.rate + "\r\n" +
        player.score;
}

function createInitialPlayer(s) {
    return {
        riichi: false,
        payment: undefined,
        score: s
    };
}

function createInitialPlayers(gameData) {
    return [
        createInitialPlayer(gameData.scores[0] * 100),
        createInitialPlayer(gameData.scores[1] * 100),
        createInitialPlayer(gameData.scores[2] * 100),
        createInitialPlayer(gameData.scores[3] * 100)
    ];
}

function parseReplay(data) {
    const defaultDoraIndicatorCount = 1;
    const akaDora = true;

    const playerCount = data.players.length;
    const players = data.players;
   
    const games = [];
    let gameCount = data.games.length;
    for (let gameId = 0; gameId < gameCount; gameId++) {
        var gameData = data.games[gameId];
        const game = {};
        game.frames = [];
        const setupFrame = {};
        setupFrame.id = 0;
        setupFrame.tilesDrawn = 13 * 4;
        setupFrame.rinshanTilesDrawn = 0;
        setupFrame.doraIndicators = defaultDoraIndicatorCount;
        setupFrame.static = { wall: gameData.wall, dice: gameData.dice, akaDora: akaDora, playerCount: playerCount, oya: gameData.oya, players: players };
        setStartingHands(setupFrame);
        setupFrame.ponds = [[], [], [], []];
        setupFrame.players = createInitialPlayers(gameData);
        game.frames.push(setupFrame);

        let previousFrame = setupFrame;
        const decisions = gameData.actions;
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
            } else if (decision === _ids.riichi) {
                const frame = createRiichiFrame(previousFrame);
                game.frames.push(frame);
                previousFrame = frame;
            } else if (decision === _ids.riichiPayment) {
                const frame = createRiichiPaymentFrame(previousFrame);
                game.frames.push(frame);
                previousFrame = frame;
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
    const frame = cloneFrame(previousFrame);
    frame.players = frame.players.slice(0);
    frame.players[who] = Object.assign({}, frame.players[who]);
    if (who === fromWho) {
        frame.players[who].announcement = announcements.tsumo;
    } else {
        frame.players[who].announcement = announcements.ron;
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
    frame.activeDiscardPlayerId = undefined;

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

function createRiichiFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.players = frame.players.slice(0);
    frame.players[frame.activePlayer] = Object.assign({}, frame.players[frame.activePlayer]);
    frame.players[frame.activePlayer].announcement = announcements.riichi;
    frame.players[frame.activePlayer].riichi = true;
    return frame;
}

function createRiichiPaymentFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.players = frame.players.slice(0);
    const activePlayer = frame.activePlayer;
    frame.players[activePlayer] = Object.assign({}, frame.players[activePlayer]);
    frame.players[activePlayer].payment = 1000;
    frame.players[activePlayer].score -= 1000;
    return frame;
}

function createTsumogiriFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.activeDiscardPlayerId = frame.activePlayer;
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
    const flipped = frame.players[frame.activePlayer].riichi && !pond.some(p => p.flipped && !p.meld);
    pond.push({ tileId: tileId, flipped: flipped });
    frame.ponds[frame.activePlayer] = pond;
    return frame;
}

function createDiscardFrame(previousFrame, indexInHand) {
    const frame = cloneFrame(previousFrame);
    frame.activeDiscardPlayerId = frame.activePlayer;
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
    const flipped = frame.players[frame.activePlayer].riichi && !pond.some(p => p.flipped && !p.meld);
    pond.push({ tileId: tileId, flipped: flipped });
    frame.ponds[frame.activePlayer] = pond;
    return frame;
}

function createDrawFrame(previousFrame) {
    const frame = cloneFrame(previousFrame);
    frame.activeDiscardPlayerId = undefined;
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

    const tileCount = frame.hands.length;
    for (let i = 0; i < tileCount; i++) {
        let x = handStartX;
        const hand = frame.hands[i];
        const tilesInHand = hand.tiles.length;
        for (let k = 0; k < tilesInHand; k++) {
            const tileId = hand.tiles[k];
            addTile(i, tileId, x, y, 0, _tilePlacement.hand);
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
        const gapOnLastTile = frame.activeDiscardPlayerId === playerId;
        createPondRow(pond.slice(0, 6), 0, playerId, pond.length <= 6 && gapOnLastTile);
        createPondRow(pond.slice(6, 12), 1, playerId, pond.length <= 12 && gapOnLastTile);
        createPondRow(pond.slice(12), 2, playerId, gapOnLastTile);
    }
}

function createPondRow(pondRow, row, playerId, gapOnLastTile) {
    const a = -(3 * tileWidth);
    var x = a + 0.5 * tileWidth;
    var y = a - 0.5 * tileHeight - row * tileHeight;
    const tileCount = pondRow.length;
    for (let column = 0; column < tileCount; column++) {
        const pondTile = pondRow[column];
        const tileId = pondTile.tileId;
        x += pondTile.flipped ? tileHeight - tileWidth : 0;
        if (gapOnLastTile && column === tileCount - 1) {
            x += 0.1;
            y -= 0.1;
        }
        if (pondTile.meld) {
            const placement = pondTile.flipped ? _tilePlacement.pondGhostFlipped : _tilePlacement.pondGhost;
            addGhostTile(playerId, tileId, x, y, 0, placement);
        } else {
            const placement = pondTile.flipped ? _tilePlacement.pondFlipped : _tilePlacement.pond;
            addTile(playerId, tileId, x, y, 0, placement);
        }
        x += tileWidth;
    }
}

function createBa(frame) {

    const playerCount = frame.static.playerCount;
    for (let i = 0; i < playerCount; i++) {
        if (frame.players[i].payment === undefined) {
            continue;
        }
        const mesh = replayContext.createBaMesh(frame.players[i].payment);
        mesh.rotateZ(Math.PI * 0.5 * i);
        mesh.translateY(-1.5);
        mesh.rotateY(Math.PI * 0.5);
        mesh.rotateZ(Math.PI * 0.5);
        replayContext.addBa(mesh);
    }
}

function createAnnouncements(frame) {
    const playerCount = frame.static.playerCount;
    for (let playerId = 0; playerId < playerCount; playerId++) {
        createAnnouncement(frame.players[playerId].announcement, playerId);
    }
    createAnnouncement(frame.announcement);
}

function createAnnouncement(announcement, playerId) {
    if (!announcement) {
        return;
    }
    if (!announcement.geometry) {
        const text = announcement.text;
        const geometry = new THREE.TextBufferGeometry(text, { font: font, size: 2, height: 0, curveSegments: 2 });
        geometry.computeBoundingBox();
        announcement.geometry = geometry;
    }
    const g = announcement.geometry;
    const boundingBox = g.boundingBox;
    const x = -0.5 * (boundingBox.max.x - boundingBox.min.x);
    const y = -0.5 * (boundingBox.max.y - boundingBox.min.y);
    const z = 2;
    const mesh = new THREE.Mesh(g, announcements.material);
    mesh.position.x = x;
    mesh.position.y = y;
    mesh.position.z = z;

    if (playerId !== undefined) {
        const p = getRotatedPlayerId(playerId);
        mesh.rotateZ(Math.PI * 0.5 * p);
        mesh.translateY(-7);
        mesh.rotateZ(Math.PI * -0.5 * p);
    }

    replayContext.addMesh(mesh, false);
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
                    const placement = isDoraIndicator ? _tilePlacement.doraIndicator : _tilePlacement.wall;
                    addTile(i, tileId, x, y, k * tileDepth, placement);
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
    tileIds.sort((x1, x2) => x2 - x1);
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
        const isFaceDown = isClosedKan && (i === 0 || i === 3);
        const isFlipped = tileId === meld.flipped;
        const placement = isFlipped ? _tilePlacement.meldedFlipped : isFaceDown ? _tilePlacement.meldedFaceDown : _tilePlacement.melded;
        addTile(playerId, tileId, x, y, 0, placement);
        if (isFlipped && meld.added !== undefined) {
            addTile(playerId, tileId, x, y + tileWidth, 0, _tilePlacement.meldedFlipped);
        }
        x -= isFlipped ? tileHeight : tileWidth;
    }
    return x;
}

function addTile(playerId, tileId, x, y, z, placement) {
    const number = numberFromTileId(tileId);
    const suit = suitFromTileId(tileId);
    const mesh = replayContext.createTileMesh(number, suit);
    addTileMesh(mesh, playerId, x, y, z, placement);
}

function addGhostTile(playerId, tileId, x, y, z, placement) {
    const number = numberFromTileId(tileId);
    const suit = suitFromTileId(tileId);
    const mesh = replayContext.createGhostTileMesh(number, suit);
    addTileMesh(mesh, playerId, x, y, z, placement);
}

function addTileMesh(mesh, playerId, x, y, z, placement) {
    const p = getRotatedPlayerId(playerId);

    var open = 0;
    var flip = 0;
    if (placement === _tilePlacement.wall) {
        open = 2;
    } else if (placement === _tilePlacement.hand) {
        if (p === 0) {
            open = -0.4;
        } else {
            open = allHandsOpen ? 0 : 3;
        }
    }  else if (placement === _tilePlacement.pondFlipped) {
        flip = 1;
    }  else if (placement === _tilePlacement.pondGhostFlipped) {
        flip = 1;
    }  else if (placement === _tilePlacement.meldedFaceDown) {
        open = 2;
    } else if (placement === _tilePlacement.meldedFlipped) {
        flip = 1;
    }
    // else if (placement === _tilePlacement.melded)
    // else if (placement === _tilePlacement.pondGhost)
    // else if (placement === _tilePlacement.pond)
    // else if (placement === _tilePlacement.doraIndicator)

    if (flip % 2 === 1) {
        const a = (tileHeight - tileWidth) / 2;
        x -= a;
        y -= a;
    }
    
    mesh.rotateZ(Math.PI * 0.5 * p);
    mesh.translateX(x);
    mesh.translateY(y);
    mesh.translateZ(z);
    mesh.rotateY(Math.PI);
    mesh.rotateX(Math.PI * 0.5 * (open - 1));
    mesh.rotateY(Math.PI * 0.5 * flip);

    replayContext.addTile(mesh);
}

function suitFromTileId(tileId) {
    return Math.floor(tileId / (9 * 4));
}

function numberFromTileId(tileId) {
    if (tileId === 4 * 4 || tileId === 16 + 9 * 4 || tileId === 16 + 9 * 4 * 2)
        return 0;
    return 1 + Math.floor(tileId % (9 * 4) / 4);
}

function getRotatedPlayerId(playerId) {
    return (playerId + 4 - _observedPlayerId) % 4;
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
