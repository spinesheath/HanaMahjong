class Analyzer {
    constructor(hand) {
        this._suitClassifiersPromise = Analyzer._dataPromise.then(data => Analyzer.createClassifiers(data, hand));
    }

    static createClassifiers(data, hand) {
        return new AnalyzerImpl(data, hand);
    }

    static getAllData() {
        const names = ["ArrangementTransitions", "ProgressiveHonorStateMachine", "SuitFirstPhase", "SuitSecondPhase0", "SuitSecondPhase1", "SuitSecondPhase2", "SuitSecondPhase3", "SuitSecondPhase4"];
        const ajax = names.map(n => Analyzer.getDataAjax(n));
        return Promise.all(ajax);
    }

    static getDataAjax(name) {
        const url = resourceUrl("analysis", name + ".txt");
        return $.get({ url: url }).then(txt => txt.split("\n").map(x => parseInt(x)));
    }

    getShantenAsync() {
        return this._suitClassifiersPromise.then(a => a.getShanten());
    }
}

Analyzer._dataPromise = Analyzer.getAllData();

class AnalyzerImpl {
    constructor(data, hand) {
        this.suit = [new SuitClassifier(data), new SuitClassifier(data), new SuitClassifier(data)];
        this.honor = new HonorClassifier(data);
        this.arrangement = new ArrangementClassifier(data);

        this.tileTypes = new Array(34).fill(0);
        const tileCount = hand.tiles.length;
        for (let i = 0; i < tileCount; i++) {
            const t = Math.floor(hand.tiles[i] / 4);
            this.tileTypes[t] += 1;
        }

        const melds = [0, 1, 2, 3].map(n => hand.melds.filter(m => m.suit === n).map(m => m.shape));
        const meldCounts = melds.map(m => m.length);

        for (let i = 0; i < 3; i++) {
            this.suit[i].setMelds(melds[i], meldCounts[i]);
        }
        const honorMeldCount = melds[3].length;
        const meldedHonors = [0, 0, 0, 0, 0, 0, 0];
        for (let i = 0; i < honorMeldCount; i++) {
            const shape = melds[i];
            if (shape < 7 + 9) {
                meldedHonors[shape - 7] = 3;
                this.honor.draw(0, 0);
                this.honor.draw(1, 0);
                this.honor.pon(2);
            } else {
                meldedHonors[shape - 7] = 4;
                this.honor.draw(0, 0);
                this.honor.draw(1, 0);
                this.honor.draw(2, 0);
                this.honor.daiminkan();
            }
        }

        for (let i = 0; i < 7; i++) {
            const count = this.tileTypes[9 * 3 + i];
            for (let j = 0; j < count; j++) {
                this.honor.draw(j, meldedHonors[i]);
            }
        }
    }

    getShanten() {
        const values = [0, 0, 0, 0];
        values[3] = this.honor.getValue();
        for (let i = 0; i < 3; i++) {
            values[i] = this.suit[i].getValue(this.tileTypes.slice(i * 9, i * 9 + 9));
        }
        return this.arrangement.classify(values) - 1;
    }
}

class ArrangementClassifier {
    constructor(data) {
        this._transitions = data[0];
    }

    classify(arrangements) {
        let current = 0;
        for (let i = 0; i < 4; i++) {
            current = this._transitions[current + arrangements[i]];
        }
        return current;
    }
}

class HonorClassifier {
    constructor(data) {
        this._transitions = data[1];
        this._current = 0;
    }

    draw(previousTiles, melded)
    {
        const actionId = previousTiles + melded + (melded & 1);
        this._current = this._transitions[this._current + actionId + 1];
        return this._transitions[this._current];
    }

    discard(tilesAfterDiscard, melded)
    {
        const actionId = 6 + tilesAfterDiscard + melded + (melded & 1);
        this._current = this._transitions[this._current + actionId];
        return this._transitions[this._current];
    }

    pon(previousTiles)
    {
        this._current = this._transitions[this._current + previousTiles + 9];
        return this._transitions[this._current];
    }

    daiminkan()
    {
        this._current = this._transitions[this._current + 13];
        return this._transitions[this._current];
    }

    getValue() {
        return this._transitions[this._current];
    }
}


class SuitClassifier {
    constructor(data) {
        this._data = data;
        this._secondPhase = data[3];
    }

    setMelds(melds, count)
    {
        this._meldCount = count;
        let current = 0;
        for (let i = 0; i < count; i++) {
            current = this._data[2][current + melds[i] + 1];
        }
        this._entry = this._data[2][current];
        this._secondPhase = this._data[3 + this._meldCount];
    }

    getValue(tiles)
    {
        let current = this._entry;
        current = this._secondPhase[current + tiles[0]];
        switch (this._meldCount) {
            case 1:
                current = this._secondPhase[current + tiles[1]];
                current = this._secondPhase[current + tiles[2]];
                current = this._secondPhase[current + tiles[3]] + 11752;
                current = this._secondPhase[current + tiles[4]] + 30650;
                current = this._secondPhase[current + tiles[5]] + 55952;
                current = this._secondPhase[current + tiles[6]] + 80078;
                current = this._secondPhase[current + tiles[7]] + 99750;
                break;
            case 2:
                current = this._secondPhase[current + tiles[1]];
                current = this._secondPhase[current + tiles[2]] + 22358;
                current = this._secondPhase[current + tiles[3]] + 54162;
                current = this._secondPhase[current + tiles[4]] + 90481;
                current = this._secondPhase[current + tiles[5]] + 120379;
                current = this._secondPhase[current + tiles[6]] + 139662;
                current = this._secondPhase[current + tiles[7]] + 150573;
                break;
            case 3:
                current = this._secondPhase[current + tiles[1]] + 24641;
                current = this._secondPhase[current + tiles[2]] + 50680;
                current = this._secondPhase[current + tiles[3]] + 76245;
                current = this._secondPhase[current + tiles[4]] + 93468;
                current = this._secondPhase[current + tiles[5]] + 102953;
                current = this._secondPhase[current + tiles[6]] + 107217;
                current = this._secondPhase[current + tiles[7]] + 108982;
                break;
            case 0:
            case 4:
                current = this._secondPhase[current + tiles[1]];
                current = this._secondPhase[current + tiles[2]];
                current = this._secondPhase[current + tiles[3]];
                current = this._secondPhase[current + tiles[4]];
                current = this._secondPhase[current + tiles[5]];
                current = this._secondPhase[current + tiles[6]];
                current = this._secondPhase[current + tiles[7]];
                break;
        }
        return this._secondPhase[current + tiles[8]];
    }
}
