var _replayId;
var _comments;

function submitComment() {
    const replayId = getValueFromComboBox("#replayId");
    const gameId = getIntFromInput("#gameId");
    const frameId = getIntFromInput("#frameId");
    const playerId = getIntFromInput("#playerId");
    const textArea = document.querySelector("#messageTextArea");
    const message = textArea.value;
    $.ajax({
        type: "POST",
        url: "/Thread/Comment",
        data: { replayId: replayId, gameId: gameId, frameId: frameId, playerId: playerId, message: message },
        success: function (data) {
            textArea.value = "";
            if (data.replayId === _replayId) {
                _comments = data.comments;
                updateComments();
            }
        }
    });
}

function updateThread(urlParams) {
    if (urlParams.v && urlParams.v !== "blame") {
        return;
    }
    
    if (!urlParams.r || !_replayIdRegex.test(urlParams.r)) {
        _replayId = undefined;
        $("#commentsDiv").html("");
        return;
    }

    if (_replayId === urlParams.r) {
        updateComments();
        return;
    }

    _replayId = urlParams.r;

    loadCommentData();
}

function loadCommentData() {
    $.ajax({
        type: "GET",
        url: "/Thread/Comments",
        data: { replayId: _replayId },
        success: function (data) {
            if (data.replayId === _replayId) {
                _comments = data.comments;
                updateComments();
            }
        }
    });
}

function updateComments() {
    const params = getUrlParamsFromInputs();
    const frameComments = _comments.filter(c => c.gameId === params.g && c.frameId === params.f && c.playerId === params.p);
    frameComments.sort((a, b) => a.timestamp > b.timestamp);
    const commentDivs = frameComments.map(c => `<div class='comment-border'><div class='comment-header'><p class='comment-user'>${c.userName}</p><p class='comment-time'>${new Date(c.timestamp).toLocaleString()}</p></div><p class='comment-message'>${c.message}</p></div>`).join("\n");
    $("#commentsDiv").html(commentDivs);
}