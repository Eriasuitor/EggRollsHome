export class Answer{
    private Ans:string;
    constructor(tmpAns = ""){
        this.Ans = tmpAns;
    }

    get ans() : string{
        return this.Ans;
    }
    set ans(tmpAns : string){
        this.Ans = tmpAns;
    }
}