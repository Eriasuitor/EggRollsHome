import {Topic} from './Topic'
export class Questionnaire{
    public QuestionnaireID: string;
    public ShortName: string;
    public FullName: string;
    public Status : number;
    public Title: string;
    public Description: string;
    public BackgroundImageUrl: string;
    public IsRealName: boolean;
    public Participants:number;
    public DueDate:Date;
    public Topics:Topic[];

    
    constructor(tmpShortName="",tmpFullName="",tmpStatus=0,tmpTitle = "",tmpDescription = "null",tmpBackgroundImageUrl = "common",tmpIsRealName = false ,tmpTopics:Topic[] = [],tmpDueDate?){
        this.ShortName = tmpShortName;
        this.FullName = tmpFullName;
        this.Status = tmpStatus;
        this.Title = tmpTitle;
        this.Description = tmpDescription;
        this.BackgroundImageUrl = tmpBackgroundImageUrl;
        this.IsRealName = tmpIsRealName;
        this.DueDate = tmpDueDate;
        this.Topics = tmpTopics;
    }

    get questionnaireID() : string{
        return this.QuestionnaireID;
    }
    set questionnaireID(tmpQuestionnaire : string){
        this.QuestionnaireID = tmpQuestionnaire;
    }

    get shortName() : string{
        return this.ShortName;
    }
    set shortName(tmpShortName : string){
        this.ShortName = tmpShortName;
    }

    get fullName() : string{
        return this.FullName;
    }
    set fullName(tmpFullName : string){
        this.FullName = tmpFullName;
    }

    get status() : number{
        return this.Status;
    }
    set status(tmpStatus : number){
        this.Status = tmpStatus;
    }

    get title() : string{
        return this.Title;
    }
    set title(tmpTitle : string){
        this.Title = tmpTitle;
    }

    get description() : string{
        return this.Description;
    }
    set description(tmpDescription : string){
        this.Description = tmpDescription;
    }

    get backgroundImageUrl() : string{
        return this.BackgroundImageUrl;
    }
    set backgroundImageUrl(tmpBackgroundImageUrl : string){
        this.BackgroundImageUrl = tmpBackgroundImageUrl;
    }

    get isRealName() : boolean{
        return this.IsRealName;
    }
    set isRealName(tmpIsRealName : boolean){
        this.IsRealName = tmpIsRealName;
    }

    get dueDate() : Date{
        return this.DueDate;
    }
    set dueDate(tmpDueDate : Date){
        this.DueDate = tmpDueDate;
    }

    get topics() : Topic[]{
        return this.Topics;
    }
    set topics(tmpTopics : Topic[]){
        this.Topics = tmpTopics;
    }

}