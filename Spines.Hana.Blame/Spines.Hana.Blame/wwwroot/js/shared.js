var font;

function initThreeJS() {
    const fontLoader = new THREE.FontLoader();
    const url = resourceUrl("fonts", "helvetiker_bold.typeface.json");
    fontLoader.load(url,
        function (f) {
            font = f;
        }
    );

    THREE.DefaultLoadingManager.onLoad = function () {
        const count = renderContexts.length;
        for (let i = 0; i < count; i++) {
            renderContexts[i].render();
        }
    };
}

function resourceUrl(folder, filename) {
    return `/resources/${folder}/${filename}`;
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

function formatString(format, args) {
    const count = args.length;
    return format.replace(/{(\d+)}/g, function (match, number) {
        return number < count ? args[number] : match;
    });
};