import { Pipe, PipeTransform} from '@angular/core';

@Pipe({name:'Num2LetterBySort'})
export class Num2LetterBySort implements PipeTransform{
    transform(value: number,language:string) : string{
        let isen:boolean = language == 'en'?true:false;
        let strASCII = String.fromCharCode(65+value);
        return strASCII.toString();
    }
}