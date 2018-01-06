function getReplays() {
    const text = document.querySelector("#browseSearchText").value;
    $.ajax({
        type: "POST",
        url: "/Browse/GetReplays",
        data: { playerName: text },
        success: updateBrowseResults
    });
}

function updateBrowseResults(data) {
    const results = data.map(r => `<div class='browse-result-div'><input type='button' class='browse-result' data-replayid='${r.id}' onclick='navigateToReplay(this.dataset.replayid)' value='${r.participants.join(" ")}\n${new Date(r.timestamp).toLocaleString()}'/></div>`).join("\n");
    $("#browseResultsDiv").html(results);
}