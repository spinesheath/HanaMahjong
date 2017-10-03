const allHandsOpen = false;
const showGhostTiles = false;

const _cameraPosition = [0, -15.5, 25];
const _lookAt = [0, -2.5, 0];

const _ambientLightColor = 0x888888;
const _pointLightColor = 0x555555;
const _pointLightPosition = [50, 50, 100];

const tileDepth = 0.78;
const tileWidth = 0.97;
const tileHeight = 1.28;
const gap = 0.2;

const _ids = {
    draw: 0,
    tsumogiri: 1,
    discardOffset: 2,
    agari: 50,
    ryuukyoku: 51,
    riichi: 52,
    riichiPayment: 60,
    dora: 53,
    rinshan: 54,
    pon: 55,
    chii: 56,
    closedKan: 57,
    calledKan: 58,
    addedKan: 59
};

const _tilePlacement = {
    wall: 0,
    doraIndicator: 1,
    pond: 2,
    pondFlipped: 3,
    pondGhost: 4,
    pondGhostFlipped: 5,
    hand: 6,
    melded: 7,
    meldedFaceDown: 8,
    meldedFlipped: 9
};

var _ranks = [
    "新人",
    "９級",
    "８級",
    "７級",
    "６級",
    "５級",
    "４級",
    "３級",
    "２級",
    "１級",
    "初段",
    "二段",
    "三初",
    "四段",
    "五初",
    "六段",
    "七段",
    "八段",
    "九段",
    "十段",
    "天鳳位"
];

const announcements = {
    riichi: { text: "riichi" },
    pon: { text: "pon" },
    chii: { text: "chii" },
    kan: { text: "kan" },
    ron: { text: "ron" },
    tsumo: { text: "tsumo" },
    ryuukyoku: { text: "ryuukyoku" },
    material: new THREE.MeshBasicMaterial({ color: 0x777777 })
};