var wwydContext;

function initWwyd() {
    window.onpopstate = onPopState;
    wwydContext = new RenderContext("wwydCanvas");
    wwydContext.setCameraPosition(0, 0, 20);
    createLights();
    initTilesAndThread();
}

function initTilesAndThread() {
    const params = new URLSearchParams(window.location.search);
    const hand = params.get("h");
    setHand(hand);
    createWwydTiles(hand);
}

function onPopState(e) {
    const hand = e.state;
    if (hand) {
        setHand(hand);
        createWwydTiles(hand);
    }
}

function changeUrl(val) {
    window.history.pushState(val, "", `//${location.host}${location.pathname}?h=${val}`);
    createWwydTiles(val);
}

function createWwydTiles(hand) {
    loadThread(hand);
    wwydContext.createTiles(() => arrangeTiles(parseWwyd(hand)));
}

function loadThread(val) {
    if (isValidHand(val)) {
        const xhr = $.ajax({
            type: "GET",
            url: "/Home/GetThread",
            data: { hand: val },
            success: function (data, textStatus, xhr2) {
                const hand = getHand();
                if (xhr2.hand === hand) {
                    $("#threadDiv").html(data);
                }
            }
        });
        xhr.hand = val;
    }
}

function arrangeTiles(tileTypes) {
    const tileTypeCount = tileTypes.length;
    for (let i = 0; i < tileTypeCount && i < 14; i++) {
        const [number, suit] = tileTypes[i];
        const mesh = wwydContext.createTileMesh(number, suit);
        const tileWidth = 0.97;
        if (i === 13) {
            mesh.translateX(i * tileWidth - 7 * tileWidth + 0.2 * tileWidth);
        }
        else {
            mesh.translateX(i * tileWidth - 7 * tileWidth);
        }
        mesh.rotation.x = Math.PI * 1.5;
        mesh.rotation.z = Math.PI * 1;
        wwydContext.scene.add(mesh);
    }
}

function createLights() {
    const light = new THREE.AmbientLight(0x999999);
    wwydContext.scene.add(light);
    const pointLight = new THREE.PointLight(0x555555);
    pointLight.position.x = 100;
    pointLight.position.y = 100;
    pointLight.position.z = 700;
    wwydContext.scene.add(pointLight);
}

function isValidHand(hand) {
    return parseWwyd(hand).length !== 0;
}

// returns an array of arrays [number, suit] ints
function parseWwyd(hand) {
    try {
        var suits = "mpsz";
        var numbers = "0123456789";
        if (hand == undefined || hand.length < 14 || hand.length > 50) {
            return [];
        }
        // var counts = new int[suits.Length * numbers.Length];
        var counts = new Array(suits.length * numbers.length);
        counts.fill(0);
        var reverse = hand.split("").reverse();
        var offset = -1;
        const characterCount = reverse.length;
        for (var i = 0; i < characterCount; i++) {
            var c = reverse[i];
            var suit = suits.indexOf(c);
            if (suit >= 0) {
                offset = 10 * suit;
                continue;
            }
            if (offset < 0) {
                return [];
            }
            var number = numbers.indexOf(c);
            if (number >= 0) {
                counts[offset + number] += 1;
                continue;
            }
            return [];
        }

        // no red 5 of 8 or 9 for honors.
        if (counts[30] + counts[38] + counts[39] > 0) {
            return [];
        }
        // at most 4 of each tile.
        if (counts.some(b => b > 4)) {
            return [];
        }
        // at most 1 red 5 each suit.
        if (counts[0] > 1 || counts[10] > 1 || counts[20] > 1) {
            return [];
        }
        // with a red 5, only 3 other 5 at most.
        if ((counts[0] > 0 || counts[10] > 0 || counts[20] > 0) && (counts[5] > 3 || counts[15] > 3 || counts[25] > 3)) {
            return [];
        }
        var sum = counts.reduce((a, b) => a + b, 0);
        if (sum !== 14) {
            return [];
        }

        var result = [];
        const length = counts.length;
        for (var m = 0; m < length; m++) {

            var j = m;
            if (m < 30) {
                if (m % 10 < 4) {
                    j = m + 1;
                } else if (m % 10 === 4) {
                    j = m - 4;
                }
            }
            const tileCount = counts[j];
            for (var k = 0; k < tileCount; k++) {
                var n = j % 10;
                var s = (j - n) / 10;
                result.push([n, s]);
            }
        }
        return result;
    }
    catch (e) {
        return [];
    }
}

function setHand(hand) {
    document.getElementById("handInput").value = hand;
}

function getHand() {
    return document.getElementById("handInput").value;
}
