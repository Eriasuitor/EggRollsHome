export class AnswersDetails{
    private ShortName:string;
    private FullName:string;
    private Department:string;
    constructor(tmpShortName ="", tmpFullName = "", tmpDepartment = ""){
        this.ShortName = tmpShortName;
        this.FullName = tmpFullName;
        this.Department = tmpDepartment;
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

    get department() : string{
        return this.Department;
    }
    set department(tmpDepartment : string){
        this.Department = tmpDepartment;
    }

}