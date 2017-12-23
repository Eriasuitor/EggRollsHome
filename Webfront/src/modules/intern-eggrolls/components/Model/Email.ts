export class Email{
    private To:string;
    private From:string;
    private Body:string;
    private Subject:string;
    constructor(tmpTo = "",tmpFrom = "",tmpBody = "",tmpSubject = ""){
        this.To = tmpTo;
        this.From = tmpFrom;
        this.Body = tmpBody;
        this.Subject = tmpSubject;
    }

    get to() : string{
        return this.To;
    }
    set to(tmpTo : string){
        this.To = tmpTo;
    }

    get from() : string{
        return this.From;
    }
    set from(tmpFrom : string){
        this.From = tmpFrom;
    }

    get body() : string{
        return this.Body;
    }
    set body(tmpBody : string){
        this.Body = tmpBody;
    }

    get subject() : string{
        return this.Subject;
    }
    set subject(tmpSubject : string){
        this.Subject = tmpSubject;
    }
}