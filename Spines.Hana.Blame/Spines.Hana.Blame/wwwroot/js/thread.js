var _comments;

function submitComment() {
    const replayId = _replayId;
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
        $("#commentsDiv").html("");
        return;
    }

    if (_comments && _replayId === urlParams.r) {
        updateComments();
        return;
    }

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
    if (!_comments) {
        return;
    }
    const params = getUrlParamsFromInputs();
    const frameComments = _comments.filter(c => c.gameId === params.g && c.frameId === params.f && c.playerId === params.p);
    frameComments.sort((a, b) => a.timestamp > b.timestamp);
    const commentDivs = frameComments.map(createCommentDiv);
    $("#commentsDiv").html(commentDivs.join(""));
}

function createCommentDiv(c) {
    var buttons = createButtons(c);
    return formatString(commentTemplate, [c.userName, new Date(c.timestamp).toLocaleString(), c.message, buttons]);
}

function createButtons(c) {
    if (c.editable) {
        return formatString(editButtonsTemplate, [c.id]);
    } else {
        return "";
    }
}

function removeComment(x) {
    const id = x.dataset.commentid;
    $.ajax({
        type: "POST",
        url: "/Thread/Remove",
        data: `id=${id}`,
        success: function (data) {
            if (data.replayId === _replayId) {
                _comments = data.comments;
                updateComments();
            }
        }
    });
}

var editButtonsTemplate =
    "<input class='comment-button' type='button' value='remove' onclick='removeComment(this)' data-commentid='{0}'/>";

var commentTemplate =
    "<div class='comment-border'>" +
        "<div class='comment-header'>" +
            "<p class='comment-user'>{0}</p>" +
            "{3}" +
            "<p class='comment-time'>{1}</p>" +
        "</div>" +
        "<p class='comment-message'>{2}</p>" +
    "</div>";

function formatString(format, args) {
    const count = args.length;
    return format.replace(/{(\d+)}/g, function (match, number) {
        return number < count ? args[number] : match;
    });
};