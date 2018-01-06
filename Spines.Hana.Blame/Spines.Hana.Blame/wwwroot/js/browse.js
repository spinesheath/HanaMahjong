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
    const results = data.map(r => `<div class='browse-result-div'><button class='browse-result' data-replayid='${r.id}' onclick='navigateToReplay(this.dataset.replayid)'><div class='browse-result-header'>${r.participants.join("   ")}</div><div class=browse-result-body>${new Date(r.timestamp).toLocaleString()}<div></button></div>`).join("\n");
    $("#browseResultsDiv").html(results);
}