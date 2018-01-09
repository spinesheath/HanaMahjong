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
    const sorted = data.sort(replaySort);
    const results = sorted.map(r => `<div class='browse-result-div'><button class='browse-result' data-replayid='${r.id}' onclick='navigateToReplay(this.dataset.replayid)'><div class='browse-result-header'>${r.participants.join("   ")}</div><div class=browse-result-body>${new Date(r.timestamp).toLocaleString()}<div></button></div>`).join("\n");
    $("#browseResultsDiv").html(results);
}

function replaySort(a, b) {
    return new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime();
}