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

var _normalIdRegex = /(\d{10}gm-\d{4}-\d{4}-[\da-f]{8})/;
var _xIdRegex = /(\d{10}gm-\d{4}-\d{4}-x[\da-f]{12})/;
var _seatRegex = /tw=(\d)/;

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
        const replayId = match[1];
        // TODO transform x format
        if (_replayIdRegex.test(replayId)) {
            input.value = "";
            return { replayId: replayId, seat: seat };
        }
    }
    
    return undefined;
}