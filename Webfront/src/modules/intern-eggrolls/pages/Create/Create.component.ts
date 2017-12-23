import { Component, OnInit, ViewChild, ElementRef, ViewChildren, QueryList, Input } from '@angular/core';
import { NegAuth, NegAjax, NegAlert, NegStorage } from '@newkit/core';
import { Router } from '@angular/router';
import { Questionnaire } from '../../components/Model/Questionnaire';
import { Topic } from '../../components/Model/Topic'
import { Option } from '../../components/Model/Option'
import { Email } from '../../components/Model/Email'
import { CreateService, UpdateService } from '../../services';
import { FormGroup, FormControl, Validators } from "@angular/forms";

@Component({
	selector: 'ntk-Create',
	templateUrl: 'Create.component.html',
	//styleUrls: ['./Create-Questionnaire.component.css'],
	styles: [require('./Create.component.css').toString()],
})

export class CreateComponent implements OnInit {
	//调查问卷Model的实体类，用于维护页面所有数据
	formControl: Questionnaire;

	//邮件Model发送实体类,用于维护邮件数据
	emailControl: Email;

	//判断标记：问卷状态是要变为草稿还是发布
	judgeSaveOrCreate: number;

	//控制标记：控制3个功能按钮：预览，保存，发布的逻辑控制，决定3个按钮能否点击
	isControl: boolean = true;

	//控制标记：是否变为更新状态的标记，用于在新建后，再次点击保存，更换请求方式，从post变为put
	isUpdate: boolean = false;

	//判断标记：是否允许创建的标记，用于标记是否允许进行新建操作，从而改变html模板上，发布按钮的disable状态
	allowCreate: boolean = true;

	//判断标记：设置背景外观的标记，用于判断是否设置问卷背景
	setAppearanceView = false;

	//控制标记：用于拖拽问题组件功能，动态更新维护问题控件时，最后一个控件的移动的特殊考虑
	lock = true;

	//用于获取当添加新的问题控件后，获得整个问题控件数组的新长度
	index: number = 0;

	//用于设置添加新的问题控件后，默认初始有多少个选项
	defaultOptionNum: number = 3;

	//用于设置时间选择控件的最早时间限制
	public dateTimePickerMin = new Date();

	//用于设置时间选择控件的默认时间设置 ---（废弃，不设置默认选择时间，置空）
	//public dateTimePickerFocused : Date = new Date();

	//用于设置发布问卷后，组装弹出的答卷链接
	questionnaireLink: string;

	//判断标记：用于判断是否弹出问卷链接模态层
	setLinkView: boolean = false;

	//题目选项的限制：最多选项数和最少选项数
	numOptionMax: number = 26;
	numOptionMin: number = 2;

	//RootUrl
	apiRootUrl: string = "http://10.16.75.27:8211/intern-eggrolls/questionnaire";


	constructor(
		private _negAuth: NegAuth,
		private _negAjax: NegAjax,
		private _negStorage: NegStorage,
		private _route: Router,
		private _createService: CreateService,
		private _updateService: UpdateService,
		private _negAlert: NegAlert,
	) {
		//console.log(this._negAuth.authData.newkitToken); // 输出用户信息
		//console.log(this._negAuth.user); // 输出用户信息
		//console.log(this._negAuth.displayName + "((((" + this._negAuth); // 输出显示名称（长名称）
		//console.log(this._negAuth.userId); // 输出用户ID（短名）

		//CKEDITOR用法：初始化
		CKEDITOR.basePath = 'http://cdn.newegg.org/ckeditor/4.7.2/';

		//初始化调查问卷Model
		this.formControl = new Questionnaire(this._negAuth.userId, this._negAuth.user.FullName);

		//初始化富文本编辑器，为空字符串
		this.formControl.description = "";
		//初始化默认背景
		this.formControl.backgroundImageUrl = "http://10.1.24.133/EaaS/Message/EggRolls_Gift.jpg";
	
		//初始化邮件Model
		this.emailControl = new Email();
	}

	ngOnInit() {
		//设置时间选择控件的默认值 ---（废弃，不设置默认选择时间，置空）
		//this.dateTimePickerFocused.setDate((new Date()).getDate() + 7);
		//this.formControl.dueDate = this.dateTimePickerFocused;

		//问卷默认是实名问卷
		this.formControl.isRealName = true;

		//判断是否是从预览界面返回本页面，当从预览界面返回时，进行数据组装
		if (this._negStorage.memory.get("CreateQuestionnaire") != undefined) {
			let strQuestionnaire = JSON.parse(this._negStorage.memory.get("CreateQuestionnaire"));
			//判断当前从预览界面返回的问卷是否已经发布，已发布，则控制标记：isUpdate置false
			if (strQuestionnaire.QuestionnaireID != undefined) {
				this.isUpdate = true;
			}
			//调用初始化页面函数，绑定页面数据
			this.initCreateView(strQuestionnaire);
			//延时执行给问题组件添加拖拽事件，为了防止在页面数据未绑定之前就为其添加拖拽功能
			setTimeout(() => {
				this.refreshFormControlList();
			}, 1000);
		}
	}

	//拖动问题后，检测viewchecked事件，做topic的题目序号更改
	ngAfterViewChecked() {
		let refreshs = document.getElementsByClassName("formList");
		for (let k = 0; k < refreshs.length; k++) {
			refreshs[k].getElementsByClassName('spanTextTitle')[0].innerHTML = String(k + 1);

		}

	}

	//动态添加问题组件
	public addFormControl(strAddTopic: string) {
		let tempTopic = new Topic(String(this.index + 1), strAddTopic);
		if (strAddTopic != "Text") {
			for (let i = 0; i < this.defaultOptionNum; i++) {
				let tempOption = new Option(i);
				let indexInitOptionPush = tempTopic.options.push(tempOption);
			}
		}
		if (strAddTopic == "Radio" || strAddTopic == "Checkbox") {
			tempTopic.isRequired = true;
		}
		let indexPush = this.formControl.topics.push(tempTopic);
		this.index = indexPush;
		setTimeout(() => {
			this.refreshFormControlList();
		}, 600);
	}
	//动态删除问题组件
	public deleteTopicControl(tempTopicNum: number) {
		for (let i = tempTopicNum; i < this.formControl.topics.length; i++) {
			this.formControl.topics[i] = this.formControl.topics[i + 1];
		}
		this.formControl.topics.pop();
	}
	//动态添加选项组件
	public addOptionControl(tempTopicNum: number, tempOptionNum: number) {
		if (this.formControl.topics[tempTopicNum].options.length + 1 <= this.numOptionMax) {
			let tempOption = new Option(tempOptionNum + 1);
			for (let i = this.formControl.topics[tempTopicNum].options.length; i > tempOptionNum + 1; i--) {
				this.formControl.topics[tempTopicNum].options[i] = this.formControl.topics[tempTopicNum].options[i - 1];
			}
			this.formControl.topics[tempTopicNum].options[tempOptionNum + 1] = tempOption;
		} else {
			this._negAlert.warn("The maximum number of options is 26");
		}
	}
	//动态删除选项组件
	public deleteOptionControl(tempTopicNum: number, tempOptionNum: number) {
		if (this.formControl.topics[tempTopicNum].options.length > this.numOptionMin) {
			for (let i = tempOptionNum; i < this.formControl.topics[tempTopicNum].options.length; i++) {
				this.formControl.topics[tempTopicNum].options[i] = this.formControl.topics[tempTopicNum].options[i + 1];
			}
			this.formControl.topics[tempTopicNum].options.pop();
		} else {
			this._negAlert.warn("The minimum number of options is 2");
		}

	}

	//刷新动态组件列表的拖动事件
	public refreshFormControlList() {
		var refreshSources = document.getElementsByClassName("formList");
		var dragElement = null;                                         // 用于存放拖动元素

		for (var i = 0; i < refreshSources.length; i++) {
			refreshSources[i].addEventListener('dragstart', function (ev) {
				dragElement = this;                                    // 用于存放拖动元素
				//this.style.backgroundColor = '#f8f8f8';                 // 设置拖动元素的背景
			}, false);

			refreshSources[i].addEventListener('dragend', function (ev) {
				//ev.target.style.backgroundColor = '#fff';               // 拖放结束还原拖动元素的背景
				ev.preventDefault();
			}, false)

			refreshSources[i].addEventListener('dragenter', function (ev) {
				if (dragElement != this) {
					this.parentNode.insertBefore(dragElement, this);     // 把拖动元素添加到当前元素的前面
				}
			}, false)

			refreshSources[i].addEventListener('dragleave', function (ev) {
				if (dragElement != this) {
					if (this.lock && (this == this.parentNode.lastElementChild || this == this.parentNode.lastChild)) {    // 当前元素时最后一个元素
						this.parentNode.appendChild(dragElement);       // 把拖动元素添加最后面
						this.lock = false;
					} else {
						//延时执行lock锁的状态改变，目的是为了防止状态改变过快，页面拖动失败
						setTimeout(() => {
							this.lock = true;
						}, 500);

					}
				}
			}, false)
		};
		document.ondragover = function (e) { e.preventDefault(); }          // 必须设置dragover阻止默认事件
		document.ondrop = function (e) { e.preventDefault(); }	// 必须设置dragover阻止默认事件
	}

	//设置外观
	public setAppearance(isSetView: boolean) {
		this.setAppearanceView = isSetView;
	}

	//弹出问卷链接
	public setLink(isSetView: boolean) {
		//this.setLinkView = isSetView;
		this.emailControl.from = this.formControl.fullName;
		this.emailControl.subject = this.formControl.title;
		this.emailControl.body = this.questionnaireLink;
		if (this.emailControl.to != "") {
			this.postEmail(this.emailControl, { useCustomErrorHandler: true });
		} else {
			this.setLinkView = false;
			setTimeout(() => {
				this._route.navigate(['/intern-eggrolls/index']);
			}, 1000);
		}
	}

	//弹出问卷链接模态层关闭按钮监听事件
	public setLinkClose() {
		this.setLinkView = false;
		setTimeout(() => {
			this._route.navigate(['/intern-eggrolls/index']);
		}, 1000);
	}

	//初始化编辑界面
	public initCreateView(tempData) {
		this.index = tempData.Topics.length;
		if (tempData.QuestionnaireID != undefined) {
			this.formControl.questionnaireID = tempData.QuestionnaireID;
		}
		this.formControl.status = tempData.Status;
		this.formControl.title = tempData.Title.toString();
		this.formControl.description = tempData.Description.toString();
		this.formControl.backgroundImageUrl = tempData.BackgroundImageUrl.toString();
		this.formControl.isRealName = tempData.IsRealName;
		if (tempData.DueDate == "null" || tempData.DueDate == null) {
			this.formControl.dueDate = null;
		} else {
			this.formControl.dueDate = new Date(tempData.DueDate);
		}
		for (let i = 0; i < tempData.Topics.length; i++) {
			let tempTopic = new Topic(tempData.Topics[i].TopicID, tempData.Topics[i].Type, tempData.Topics[i].IsRequired, tempData.Topics[i].Limited, undefined, tempData.Topics[i].TopicTitle);
			for (let j = 0; j < tempData.Topics[i].Options.length; j++) {
				let tempOption = new Option(tempData.Topics[i].Options[j].OptionID, tempData.Topics[i].Options[j].OptionTitle);
				tempTopic.options.push(tempOption);
			}
			this.formControl.topics.push(tempTopic);
		}
		this._negStorage.memory.remove("EditQuestionnaire");
	}


	//跳转到预览界面
	public priviewTheQuestionnaire() {
		this.assemblyQuestionnaire();
		setTimeout(() => {
			let tempStr = JSON.stringify(this.formControl);
			this._negStorage.memory.set("PreQuestionnaire", [tempStr, "create"]);
			this._route.navigate(['/intern-eggrolls/preview']);
		}, 400);
	}

	//对题目和选项排序
	public sortTheQuestionnaire(m: Topic, n: Topic): number {
		if (parseInt(m.topicID) < parseInt(n.topicID)) {
			return -1;
		} else if (parseInt(m.topicID) > parseInt(n.topicID)) {
			return 1;
		} else {
			return 0;
		}
	}

	//输入数据，由全角转换为半角
	public fullWidth2HalfWidth(tempInput){
		let result = tempInput.replace(/[０１２３４５６７８９]/g, function (v) { return v.charCodeAt(0) - 65296; });
		return parseInt(result);
	}

	//判断页面填写是否合法
	public judgeNormal(): boolean {
		let isNormal: boolean = false;
		if (this.formControl.title != "" && this.formControl.title.replace(/\s+/g, "") != "") {
			let k = 0;
			for (let i = 0; i < this.formControl.topics.length; i++) {
				if (this.formControl.topics[i].topicTitle == undefined) {
					this.formControl.topics[i].topicTitle = "";
				}
				//全角转半角
				this.formControl.topics[i].limited = this.fullWidth2HalfWidth(this.formControl.topics[i].limited.toString());

				if (this.formControl.topics[i].options.length >= this.formControl.topics[i].limited && this.formControl.topics[i].limited >= 0 && !isNaN(this.formControl.topics[i].limited) && this.formControl.topics[i].topicTitle != "") {
					k++;
				}
			}
			if (k == this.formControl.topics.length || this.formControl.topics.length == 0) {
				if (this.formControl.description.length > 3800) {
					this._negAlert.warn("Description limit 1000 characters, beyond the automatic interception, please note");
				} else {
					isNormal = true;
				}
			}
		}
		return isNormal;
	}

	//组装页面所有信息成Questionnaire对象
	public assemblyQuestionnaire() {
		this.formControl.title = this.formControl.title.replace(/(^\s*)/g, "");
		this.formControl.title = this.formControl.title.replace(/(\s*$)/g, "");
		let labelOptions = document.getElementsByClassName('labelOption');
		let spanTopics = document.getElementsByClassName('spanTextTitle');

		//组装Topic
		for (let i = 0; i < this.formControl.topics.length; i++) {
			let sortArrayIndex = parseInt(spanTopics[i].attributes["title"].value) + 1;
			this.formControl.topics[i].topicID = sortArrayIndex.toString();
		}
		//排序Topic
		this.formControl.topics.sort(this.sortTheQuestionnaire);

		//组装Option
		let k = 0;
		for (let i = 0; i < this.formControl.topics.length; i++) {

			for (let j = 0; j < this.formControl.topics[i].options.length; j++) {
				this.formControl.topics[i].options[j].optionID = labelOptions[k].getElementsByTagName('span')[0].innerHTML;
				this.formControl.topics[i].options[j].topicID = this.formControl.topics[i].topicID;
				k++;
			}
		}

	}

	//发布和保存功能：判断进行新建还是修改操作
	public saveAndCreateQuestionnaire(numStatus: number) {
		this.judgeSaveOrCreate = numStatus;
		if (this.judgeNormal()) {
			if (numStatus == 1) {
				if (this.formControl.dueDate != undefined) {
					if (this.formControl.dueDate >= this.dateTimePickerMin) {
						if (this.isUpdate) {
							this.updateQuestionnaire(this.formControl.questionnaireID);
						} else {
							this.createQuestionnaire(numStatus);
						}
					} else {
						this._negAlert.error("Please check if your Deadline is legal！");
					}

				} else {
					this._negAlert.error("Please check your fill is valid(Such as: Deadline)！");
				}
			} else {
				if (this.formControl.dueDate == undefined) {
					this.formControl.dueDate = null;
				}
				if (this.isUpdate) {
					this.updateQuestionnaire(this.formControl.questionnaireID);
				} else {
					this.createQuestionnaire(numStatus);
				}
			}

		} else {
			this._negAlert.error("Please check your fill is valid(Such as:Limited, Questionnaire Title or Topic Title！！");
		}


	}

	//对应修改功能
	public updateQuestionnaire(tmpID: string) {
		if (tmpID != "") {
			this.assemblyQuestionnaire();
			this.formControl.status = this.judgeSaveOrCreate;

			this.isControl = false;
			this.putTheQuestionnaire(this.formControl, {});
		}
	}

	//对应发布功能：新建一张调查问卷
	public createQuestionnaire(numStatus: number) {
		this.assemblyQuestionnaire();
		this.formControl.status = numStatus;
		this.isControl = false;
		this.postTheQuestionnaire(this.formControl, {});
	}

	//post请求调查问卷：新建调查问卷的service方法调用
	public postTheQuestionnaire(postQuestionnaire: Questionnaire, postHeader) {
		this._createService.postQuestionnaire(postQuestionnaire, postHeader).then(({ data }) => {
			let resData = data.Questionnaire;

			if (resData.QuestionnaireID != null) {
				//当前编辑的问卷ID为提交成功后的ID
				this.formControl.questionnaireID = resData.QuestionnaireID;
				this.isControl = true;

				//this._negAlert.success("发布成功！！！");
				if (this.judgeSaveOrCreate == 0) {
					this._negAlert.success("Saved successfully！");
					this.isControl = true;
					this.isUpdate = true;
					this.allowCreate = true;
					this.setLinkView = false;
				} else {
					//this._negAlert.success("Publish successfully！！！");
					this.isControl = true;
					this.isUpdate = true;
					this.allowCreate = false;
					this.setLinkView = true;
				}
				//弹出问卷链接标记置true
				this.questionnaireLink = this.apiRootUrl + "?questionnaireID=" + this.formControl.questionnaireID;

			} else {
				this.isControl = true;

				if (this.judgeSaveOrCreate == 0) {
					this._negAlert.error("Saved failed！");
					this.isControl = true;
					this.isUpdate = false;
					this.allowCreate = true;
				} else {
					this._negAlert.error("Publish failed！");
					this.isControl = true;
					this.isUpdate = false;
					this.allowCreate = true;
				}
			}
		},
			error => this._negAlert.error("PUT Operation error！"));

	}



	//put请求调查问卷：更新调查问卷的service方法调用
	public putTheQuestionnaire(putQuestionnaire: Questionnaire, putHeader) {
		this._updateService.putQuestionnaire(putQuestionnaire, putHeader).then(({ data }) => {
			let resData = data;
			if (resData.EffectRows != "0") {
				this.isControl = true;
				this.isUpdate = true;
				//this._negAlert.success("保存成功！！！");
				if (this.judgeSaveOrCreate == 0) {
					this.isUpdate = true;
					this.isControl = true;
					this._negAlert.success("Saved successfully！");
					this.setLinkView = false;
					this.allowCreate = true;
				} else {
					this.isUpdate = true;
					this.isControl = true;
					//this._negAlert.success("Publish successfully！！！");
					this.setLinkView = true;
					this.allowCreate = false;
				}
				//弹出问卷链接标记置true
				this.questionnaireLink = this.apiRootUrl + "?questionnaireID=" + this.formControl.questionnaireID;
			} else {
				this.isControl = true;
				this.isUpdate = true;
				if (this.judgeSaveOrCreate == 0) {
					this.isUpdate = true;
					this.isControl = true;
					this._negAlert.error("Saved failed！");
					this.setLinkView = false;
					this.allowCreate = true;
				} else {
					this.isUpdate = true;
					this.isControl = true;
					this._negAlert.error("Publish failed！");
					this.setLinkView = false;
					this.allowCreate = true;
				}
			}
		},
			error => this._negAlert.error("PUT Operation error！"));
	}

	//post请求邮件：新建邮件发送的service方法调用
	public postEmail(postEmail: Email, postHeader) {
		this._createService.postEmail(postEmail, postHeader).then(({ data }) => {
			let resData = data;
			if (resData.IsSendSuccess) {
				this._negAlert.success("Mail sent successfully！");
				this.setLinkView = false;
				setTimeout(() => {
					this._route.navigate(['/intern-eggrolls/index']);
				}, 1000);
			} else {
				this._negAlert.error("Mail delivery failed！")
				this.setLinkView = true;
			}
		},
			error => {
				if (error.status == 500) {
					this._negAlert.error("Mail delivery failed！");
					this._negAlert.error("Please check the email address is correct！");
				} else {
					this._negAlert.error("Mail delivery failed！");
				}
			});
	}
}



//调试用函数：输出传入对象的类名
function getObjectClass(obj) {
	if (obj && obj.constructor && obj.constructor.toString) {
		var arr = obj.constructor.toString().match(
			/function\s*(\w+)/);

		if (arr && arr.length == 2) {
			return arr[1];
		}
	}
	return undefined;
}








