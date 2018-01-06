function snitch() {
    const error = $("#snitchError");
    error.html("");
    const replayId = getSnitchedReplayId();
    if (!replayId) {
        error.html("invalid id");
        return;
    }
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
            navigateToReplay(replayId);
            error.html("");
        }
    });
}

function getSnitchedReplayId() {
    const input = document.querySelector("#snitchReplayIdText");
    const value = input.value;
    if (!value) {
        return undefined;
    }
    if (_replayIdRegex.test(value)) {
        input.value = "";
        return value;
    }
    return undefined;
}