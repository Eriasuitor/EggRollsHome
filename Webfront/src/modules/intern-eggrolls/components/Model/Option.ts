export class Option {
    public OptionID: string;
    public OptionTitle: string;
    public TopicID: string
    public IsAnswer: boolean;
    constructor(tmpOptionId, tmpOptionTitle = "") {
        this.OptionID = tmpOptionId;
        this.OptionTitle = tmpOptionTitle;
    }

    get topicID(): string {
        return this.TopicID;
    }
    set topicID(tmpTopicId: string) {
        this.TopicID = tmpTopicId;
    }

    get optionID(): string {
        return this.OptionID;
    }
    set optionID(tmpOptionId: string) {
        this.OptionID = tmpOptionId;
    }

    get optionTitle(): string {
        return this.OptionTitle;
    }
    set optionTitle(tmpOptionTitle: string) {
        this.OptionTitle = tmpOptionTitle;
    }

    get isAnswer(): boolean {
        return this.IsAnswer;
    }
    set isAnswer(tmpIsAnswer: boolean) {
        this.IsAnswer = tmpIsAnswer;
    }
}