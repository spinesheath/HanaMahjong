// urlParams:
// v: selected partial view. blame if undefined.
// r: replayId for blame
// g: gameId for blame
// f: frameId for blame
// p: playerId for blame

var _replayId;

function initView() {
    const params = getUrlParams();
    if (params.r) {
        showView("blame");
    } else {
        showView("browse");
    }
}

function navigateToReplay(replayId, seat) {
    showView("blame");
    const params = getUrlParams();
    params.v = undefined;
    params.r = replayId;
    params.p = seat;
    setFrameInputData(params);
    setBrowserHistory(params);
    onReplayIdChanged(replayId, seat);
}

function onPopstate(e) {
    const urlParams = e.state;
    showView(urlParams && urlParams.v);
    replayOnPopState(urlParams);
}

// displays the partial view with the given contentName. blame if undefined. Also updates browser history.
function navigateTo(contentName) {
    showView(contentName);
    if (contentName === "blame" || !contentName) {
        const replayParams = getUrlParamsFromInputs();
        setBrowserHistory(replayParams);
    } else {
        setBrowserHistory({ v: contentName });
    }
}

function updateHistory(urlParams) {
    if (urlParams && urlParams.r) {
        updateUrlParams(urlParams);
    } else {
        updateUrlParams({ r: undefined, p: undefined, g: undefined, f: undefined });
    }
}

function onReplayIdChanged(replayId, seat) {
    const d = { r: replayId, p: seat, g: 0, f: 0 };
    loadReplayAndThread(d);
}

function loadReplayAndThread(d) {
    if (!d) {
        d = getUrlParams();
    }
    _replayId = d.r;
    loadReplay(d);
    updateThread(d);
}

function getUrlParamsFromInputs() {
    const replayId = getUrlParams().r;
    const playerId = getIntFromInput("#playerId");
    const game = getIntFromInput("#gameId");
    const frame = getIntFromInput("#frameId");
    return { r: replayId, p: playerId, g: game, f: frame };
}

function setFrameInputData(urlParams) {
    const wrapped = wrapFrameInputValues(urlParams);
    setValueToInput("#playerId", wrapped.p);
    setValueToInput("#gameId", wrapped.g);
    setValueToInput("#frameId", wrapped.f);
}

function replayOnPopState(state) {
    if (state) {
        setFrameInputData(state);
        const newReplayId = getUrlParams().r;
        const d = getUrlParamsFromInputs();
        if (_replayId !== newReplayId) {
            _replayId = newReplayId;
            loadReplayAndThread(d);
        }

        replayContext.createTiles(() => arrange());
        updateThread(d);
    }
}

function onFrameChanged() {
    const urlParams = getUrlParamsFromInputs();
    const wrapped = wrapFrameInputValues(urlParams);
    setFrameInputData(wrapped);
    updateHistory(wrapped);
    replayContext.createTiles(() => arrange());
    updateThread(wrapped);
}

function getIntFromParams(params, key) {
    return parseInt(params.get(key)) || 0;
}

function getStringFromParams(params, key) {
    if (!params.has(key))
        return undefined;
    return params.get(key);
}

function getUrlParams() {
    const params = new URLSearchParams(window.location.search);
    const view = getIntFromParams(params, "v");
    const replayId = getStringFromParams(params, "r");
    const playerId = getIntFromParams(params, "p");
    const game = getIntFromParams(params, "g");
    const frame = getIntFromParams(params, "f");
    return {v: view, r: replayId, p: playerId, g: game, f: frame };
}

function getBrowserHistoryData() {
    return window.history.state;
}

// data must be an object with property names matching the keys used in the url parameters
function setBrowserHistory(data) {
    const previousData = getBrowserHistoryData();
    const previousParams = _getHistoryParameterString(previousData);
    const params = _getHistoryParameterString(data);
    if (previousParams === params) {
        return;
    }
    const url = `//${location.host}${location.pathname}${params}`;
    window.history.pushState(data, "", url);
}

function _getHistoryParameterString(data) {
    if (data) {
        const keys = Object.keys(data).filter(k => data[k]);
        if (keys.length === 0) {
            return "";
        }
        keys.sort();
        const x = keys.map(k => k + "=" + data[k]).join("&");
        return `?${x}`;
    } else {
        return "";
    }
}

// keeps existing parameters, adds new ones and overwrites parameters with identical keys
// keys with undefined values will be removed
function updateUrlParams(data) {
    const current = Object.assign({}, getBrowserHistoryData());
    const keys = Object.keys(data);
    const count = keys.length;
    for (let i = 0; i < count; i++) {
        const key = keys[i];
        const value = data[key];
        if (value) {
            current[key] = value;
        } else {
            delete current[key];
        }
    }
    setBrowserHistory(current);
}

function getIntFromInput(id) {
    const input = document.querySelector(id);
    return input.value ? parseInt(input.value) : 0;
}

function setValueToInput(id, value) {
    const isDefined = value | value === 0;
    const x = isDefined ? value : "";
    document.querySelector(id).value = x;
}

// displays the partial view with the given contentName. blame if undefined.
function showView(target) {
    const contentName = target || "blame";
    const contents = document.getElementsByClassName("content");
    const count = contents.length;
    for (let i = 0; i < count; i++) {
        const content = contents[i];
        content.hidden = content.dataset.content !== contentName;
    }
}