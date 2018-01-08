var _normalIdRegex = /(\d{10}gm-[\da-f]{4}-[\da-f]{4}-[\da-f]{8})/;
var _xIdRegex = /(\d{10}gm)(-[\da-f]{4}-[\da-f]{4}-)x([\da-f]{4})([\da-f]{4})([\da-f]{4})/;
var _seatRegex = /tw=(\d)/;
var _tt =
[
    22136, 52719, 55146, 42104, 59591, 46934, 9248, 28891, 49597,
    52974, 62844, 4015, 18311, 50730, 43056, 17939, 64838, 38145,
    27008, 39128, 35652, 63407, 65535, 23473, 35164, 55230, 27536,
    4386, 64920, 29075, 42617, 17294, 18868, 2081
];

function snitch() {
    const error = $("#snitchError");
    error.html("");
    const data = getSnitchedData();
    if (!data) {
        error.html("invalid id");
        return;
    }
    const replayId = data.replayId;
    const seat = data.seat;
    const button = document.querySelector("#snitchReplayIdButton");
    button.disabled = true;
    $.ajax({
        type: "GET",
        url: "/Api/Snitch",
        data: `replayId=${replayId}`,
        dataType: "text",
        error: function () {
            button.disabled = false;
            error.html("upload failed");
        },
        success: function () {
            button.disabled = false;
            navigateToReplay(replayId, seat);
            error.html("");
        }
    });
}

function getSnitchedData() {
    const input = document.querySelector("#snitchReplayIdText");
    const value = input.value;
    if (!value) {
        return undefined;
    }

    const seatMatch = _seatRegex.exec(value);
    const seat = seatMatch && seatMatch[1];

    const match = _normalIdRegex.exec(value);
    if (match) {
        const replayId = match[1];
        input.value = "";
        return { replayId: replayId, seat: seat };
    }

    const xMatch = _xIdRegex.exec(value);
    if (xMatch) {
        const date = xMatch[1];
        const room = xMatch[2];
        const a = parseInt(xMatch[3], 16);
        const b = parseInt(xMatch[4], 16);
        const c = parseInt(xMatch[5], 16);
        let d = 0;
        if (date > "2010041111gm") {
            const x = parseInt(`3${date.substring(4, 10)}`);
            const y = parseInt(date.substring(9, 10));
            d = x % (33 - y);
        }
        const postfix1 = (a ^ b ^ _tt[d]).toString(16).padStart(4, "0");
        const postfix2 = (b ^ c ^ _tt[d] ^ _tt[d + 1]).toString(16).padStart(4, "0");
        const replayId = date + room + postfix1 + postfix2;

        if (_replayIdRegex.test(replayId)) {
            input.value = "";
            return { replayId: replayId, seat: seat };
        }
    }
    
    return undefined;
}