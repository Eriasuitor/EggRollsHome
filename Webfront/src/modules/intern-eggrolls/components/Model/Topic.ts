import {Option} from './Option'
import {Answer} from './Answer'
export class Topic{
    private TopicID: string;
    private Type: string;
    private IsRequired:boolean;
    private Limited:number;
    private TopicTitle:string;
    private Options:Option[];
    private Answer:string;
    constructor(tmpTopicId = '0',tmpType = "Radio",tmpIsRequired = false,tmpLimited = 0,tmpOptions:Option[] = [],tmpTopicTitle?){
        this.TopicID = tmpTopicId;
        this.Type = tmpType;
        this.IsRequired = tmpIsRequired;
        this.Limited = tmpLimited;
        this.TopicTitle = tmpTopicTitle;
        this.Options = tmpOptions;
    }
    get topicID() : string{
        return this.TopicID;
    }
    set topicID(tmpTopicID : string){
        this.TopicID = tmpTopicID;
    }

    get type() : string{
        return this.Type;
    }
    set type(tmpType : string){
        this.Type = tmpType;
    }

    get isRequired() : boolean{
        return this.IsRequired;
    }
    set isRequired(tmpIsRequired : boolean){
        this.IsRequired = tmpIsRequired;
    }

    get topicTitle() : string{
        return this.TopicTitle;
    }
    set topicTitle(tmpTopicTitle : string){
        this.TopicTitle = tmpTopicTitle;
    }

    get options() : Option[]{
        return this.Options;
    }
    set options(tmpOptions : Option[]){
        this.Options = tmpOptions;
    }

    get limited() : number{
        return this.Limited;
    }
    set limited(tmpLimited : number){
        this.Limited = tmpLimited;
    }

    get answer() : string{
        return this.Answer;
    }
    set answer(tmpAnswers : string){
        this.Answer = tmpAnswers;
    }

    

}