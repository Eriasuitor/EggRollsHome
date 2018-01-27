import { Component, OnInit, ViewChild, ElementRef, ViewChildren, QueryList, Input } from '@angular/core';
import { Router } from '@angular/router';

import { NegAuth, NegAjax, NegAlert, NegStorage, NegTranslate, NegMultiTab } from '@newkit/core';

import { Questionnaire } from '../../components/Model/Questionnaire';
import { Topic } from '../../components/Model/Topic'
import { Option } from '../../components/Model/Option'
import { Email } from '../../components/Model/Email'

import { QuestionnaireService } from '../../services/QuestionnaireService'

import { Environment } from '../../config/Environment'

import { FormGroup, FormControl, Validators } from "@angular/forms";

@Component({
	selector: 'ntk-Create',
	templateUrl: 'Create.component.html',
	styles: [require('./Create.component.css').toString()],
})

export class CreateComponent implements OnInit {
	formControl: Questionnaire;
	valve = false;
	allowCreate: boolean = true;
	setAppearanceView = false;
	defaultOptionNum: number = 3;
	public dateTimePickerMin = new Date();
	numOptionMax: number = 26;
	numOptionMin: number = 1;
	public shosen = 'background-color: rgb(162, 210, 160);'
	apiRootUrl: string = "http://10.16.75.27:8211/eggrolls/questionnaire";
	public imgUrls: string[] = ['http://10.1.24.133/EaaS/Message/EggRolls_Ancient.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Ancient2.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Ancient3.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_ArtisticColor.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Cartoon.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Cat.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Cats.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_ChineseNewYear.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_ChineseNewYear2.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Christmas4.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Cloud.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Flower.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Flower2.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Food.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Gift.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Island.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Lattice.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_NewYear.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Paper.png', 'http://10.1.24.133/EaaS/Message/EggRolls_Peak.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_PUBG.png', 'http://10.1.24.133/EaaS/Message/EggRolls_Simple.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Simple2.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Simple3.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Summer.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Surf.jpg', 'http://10.1.24.133/EaaS/Message/EggRolls_Watercolor.jpg']
	public selectedOption = 21
	public pubSuccess = false
	public sendSuccess = false
	public environmentUrl: string
	self: CreateComponent;

	constructor(
		private _negAuth: NegAuth,
		private _negAjax: NegAjax,
		private _negStorage: NegStorage,
		private _route: Router,
		private _negAlert: NegAlert,
		private _negTranslate: NegTranslate,
		private questionnaireService: QuestionnaireService,
		private _environment: Environment,
		private negMultiTab: NegMultiTab
	) {
		CKEDITOR.basePath = 'http://cdn.newegg.org/ckeditor/4.7.2/';
		this.formControl = new Questionnaire(this._negAuth.userId, this._negAuth.user.FullName);
		this.formControl.description = "";
		this.environmentUrl = this._environment.getEnvironmentUrl()
	}

	ngOnInit() {
		this.negMultiTab.setCurrentTabName('Egg Rolls')
		this.formControl.isRealName = true;
		this.formControl.MailTo = ""
		self = this
		if (this._negStorage.memory.get("eID") != undefined && this._negStorage.memory.get("eID") != null) {
			this.questionnaireService.OnGet(this._negStorage.memory.get("eID"))
				.then(({ data }) => {
					if (data.Succeeded == true) {
						this.valve = true
						this.formControl = data.Questionnaire
						this.formControl.DueDate = this.getStanderdDate(this.formControl.DueDate)
					}
					else {
						this._negAlert.warn(this._negTranslate.get('new.wrong.getWrong'))
					}
					setTimeout(this.refreshFormControlList, 20)
					setTimeout(() => {
						this.setBackImg()
					}, 20)
				},
				error => {
					this._negAlert.warn(this._negTranslate.get('new.wrong.getWrong'))
					setTimeout(() => {
						this.setBackImg()
					}, 20)
				})
			this._negStorage.memory.remove("eID")
		}
		else if (this._negStorage.memory.get("cID") != undefined && this._negStorage.memory.get("cID") != null) {
			this.questionnaireService.OnGet(this._negStorage.memory.get("cID"))
				.then(({ data }) => {
					if (data.Succeeded == true) {
						this.valve = true
						this.formControl = data.Questionnaire
						this.formControl.QuestionnaireID = 0
						this.formControl.DueDate = this.getStanderdDate(this.formControl.DueDate)
					}
					else {
						this._negAlert.warn(this._negTranslate.get('new.wrong.getWrong'))
					}
					setTimeout(this.refreshFormControlList, 20)
					setTimeout(() => {
						this.setBackImg()
					}, 20)
				},
				error => {
					this._negAlert.warn(this._negTranslate.get('new.wrong.getWrong'))
					setTimeout(() => {
						this.setBackImg()
					}, 20)
				})
			this._negStorage.memory.remove("cID")
		}

		//设置时间选择控件的默认值 ---（废弃，不设置默认选择时间，置空）
		//this.dateTimePickerFocused.setDate((new Date()).getDate() + 7);
		//this.formControl.dueDate = this.dateTimePickerFocused;
		//问卷默认是实名问卷

		//判断是否是从预览界面返回本页面，当从预览界面返回时，进行数据组装
		else if (this._negStorage.memory.get("CreateQuestionnaire") != undefined && this._negStorage.memory.get("CreateQuestionnaire") != null) {
			let strQuestionnaire = JSON.parse(this._negStorage.memory.get("CreateQuestionnaire"));
			//判断当前从预览界面返回的问卷是否已经发布，已发布，则控制标记：isUpdate置false
			if (strQuestionnaire.QuestionnaireID != undefined) {
				this.isUpdate = true;
			}
			this.valve = true
			this.formControl = strQuestionnaire
			this.formControl.DueDate = this.getStanderdDate(this.formControl.DueDate)
			setTimeout(() => {
				this.refreshFormControlList();
			}, 20);
			setTimeout(() => {
				this.setBackImg()
			}, 20)
		}
		else {
			this.valve = true
			setTimeout(() => {
				this.setBackImg()
			}, 20)
		}

		this._negTranslate.set('new', {
			'en-us': {
				questionnaire: {
					titleTip: 'Click here to edit the title of the questionnaire',
					realName: 'Real name',
					deadline: 'Deadline',
					descriptionTip: 'Please edit the description of the questionnaire below',
					language: 'en'
				},
				topic: {
					noTopicTip: 'Click the button on the right edge to add topic',
					singleChoice: 'Single choice',
					multiChoice: 'Multiple choice',
					subjQuestion: 'Subjective question',
					backgImg: 'Background image',
					limit: 'Limit',
					required: 'Required',
					topicTip: 'Edit the topic title here',
					optionTip: 'Edit the option here',
					limitTip1: ', have to choose ',
					limitTip2: ' options',
					requiredTip: ' *',
					textTip: 'The respondent will answer here',
					mailTo: 'Mail To',
					publish: 'Publish',
					save: 'Save',
					preview: 'Preview',
					noImg: 'No background image'
				},
				wrong: {
					queTitle: 'Please add a title',
					deadline: 'Please check the deadline',
					topicTitle: 'Please fill in the topic title',
					optionTitle: 'Please fill in the option title',
					smaller: 'The number is smaller than 2',
					greater: 'The number is greater than the number of options',
					format: 'Wrong number format',
					noTopic: 'Please do as said on the left',
					optionTooMuch: 'The maximum number of options is 26',
					optionTooSmall: 'The minimum number of options is 1',
					getWrong: 'Sometging wrong when get questionnaire, please contact E.T. for help',
					formatWrong: 'Please check the correctness of the part with the badge.',
					pubFailed: 'Questionnaire failed to publish',
					pubInf: 'Something wrong when publish the questionnaire, please check your questionnaire or contact E.T. for help',
					sendFailed: 'Mail delivery failed',
					sendInf: 'Something wrong when send e-mail, please contact E.T. for help',
					descriptionTooLong: 'Description exceeds the maximum length， please shorten it.',
					optionLessThanLimit:'The number of options can not less than the value of limit'
				},
				transfer: {
					saveRight: 'Save successfully',
					saveFailed: 'Save failed, please contact E.T. for help',
					publishRight: 'Publish success',
					publishFailed: 'Publish failed, please try again or contact E.T. for help',
					publishing: 'Publishing questionnaire',
					published: 'Questionnaire released successfully',
					sending: 'Sending e-mail',
					sent: 'Mail sent successfully',
					answerLink: 'Answer link',
					back: 'Return to home page'
				}
			},
			'zh-cn': {
				questionnaire: {
					titleTip: '点击此处编辑问卷标题',
					realName: '实名',
					deadline: '截止时间：',
					descriptionTip: '请在下方编写问卷描述信息',
					language: 'zh-cn'
				},
				topic: {
					noTopicTip: '点击下方按钮以添加题目',
					singleChoice: '单选',
					multiChoice: '多选',
					subjQuestion: '问答题',
					backgImg: '背景图片',
					limit: '限选 ',
					required: '必填',
					topicTip: '请在此处编辑题干',
					optionTip: '请在此处编辑选项',
					limitTip1: '， 必选 ',
					limitTip2: ' 个',
					requiredTip: ' *',
					textTip: '答题人将在此处作答',
					mailTo: 'Mail To',
					publish: '发布',
					save: '保存',
					preview: '预览',
					noImg: '无背景图片'
				},
				wrong: {
					queTitle: '请填写标题',
					deadline: '请检查截止时间',
					topicTitle: '请填写题干',
					optionTitle: '请填写选项',
					smaller: '选择数量限制不得小于2',
					greater: '选择数量限制不得大于本题选项个数',
					format: '错误的数字格式',
					noTopic: '请照着左边说的做',
					optionTooMuch: '选项个数最多为26个',
					optionTooSmall: '选项个数最少为1个',
					getWrong: '获取问卷时出现问题，请联系E.T.寻求帮助',
					formatWrong: '请检查带有徽章提示部分的正确性',
					pubFailed: '问卷发布失败',
					pubInf: '发布问卷时出现问题，请检查您的问卷或联系E.T.寻求帮助',
					sendFailed: '邮件发送失败',
					sendInf: '发送邮件时出现问题，请联系E.T.寻求帮助',
					descriptionTooLong: '描述大于最大长度，请进行删减',
					optionLessThanLimit:'选项个数不得小于Limit所设值'				
				},
				transfer: {
					saveRight: '保存成功',
					saveFailed: '保存失失败，请联系E.T.寻求帮助',
					publishRight: '发布成功',
					publishFailed: '发布失败，请重试或联系E.T.寻求帮助',
					publishing: '正在发布问卷',
					published: '问卷发布成功',
					sending: '正在发送邮件',
					sent: '邮件发送成功',
					answerLink: '答题链接',
					back: '返回首页'
				}
			},
			'zh-tw': {
				questionnaire: {
					titleTip: 'Click here to edit the title of the questionnaire',
					realName: 'Real name',
					deadline: 'Deadline',
					descriptionTip: 'Please edit the description of the questionnaire below',
					language: 'en'
				},
				topic: {
					noTopicTip: 'Click the button on the right edge to add a topic',
					singleChoice: 'Single choice',
					multiChoice: 'Multiple choice',
					subjQuestion: 'Subjective question',
					backgImg: 'Background image',
					limit: 'Limit',
					required: 'Required',
					topicTip: 'Edit the topic title here',
					optionTip: 'Edit the option here',
					limitTip1: ', have to choose ',
					limitTip2: ' options',
					requiredTip: ' *',
					textTip: 'The respondent will answer here',
					mailTo: 'mailTo',
					publish: 'Publish',
					save: 'Save',
					preview: 'Preview',
					noImg: 'No background image'
				},
				wrong: {
					queTitle: '请填写标题',
					deadline: '请检查截止时间',
					topicTitle: '请填写题干',
					optionTitle: '请填写选项',
					smaller: '数量限制不得小于2',
					greater: '数量限制不得大于本题选项个数',
					format: '错误的数字格式',
					noTopic: '请照着左边说的做',
					optionTooMuch: 'The maximum number of options is 26',
					optionTooSmall: 'The minimum number of options is 1',
					getWrong: 'Sometging wrong when get questionnaire, please contact E.T. for help',
					formatWrong: 'Please check the correctness of the part with the badge.',
					pubFailed: 'Publish failed',
					pubInf: 'Something wrong when publish the questionnaire, please check your questionnaire or contact E.T. for help',
					sendFailed: 'Send failed',
					sendInf: 'Something wrong when send e-mail, please contact E.T. for help',
					descriptionTooLong: '描述大于最大长度,请进行删减',
					optionLessThanLimit:'选项个数不得小于Limit所设值'			
				},
				transfer: {
					saveRight: 'Save successfully',
					saveFailed: 'Save failed, please review your questionnaire',
					publishRight: '发布成功',
					publishFailed: '发布失败，请重试或联系E.T.寻求帮助',
					publishing: 'Publishing questionnaire',
					published: 'Published successfully',
					sending: 'Sending e-mail',
					sent: 'Sent successfully',
					answerLink: 'Answer link',
					back: '返回首页'
				}
			}
		}
		);
	}

	public setBackImg() {
		this.selectedOption = this.imgUrls.indexOf(this.formControl.BackgroundImageUrl)
		if (this.selectedOption != -1) {
			document.getElementById('img' + this.selectedOption).style.backgroundColor = 'rgb(162, 210, 160)'
		}
	}

	public addFormControl(strAddTopic: string) {
		var tempTopic
		if (this.formControl.Topics.length == 0) {
			tempTopic = new Topic(1, strAddTopic);
		}
		else {
			tempTopic = new Topic(this.formControl.Topics[this.formControl.Topics.length - 1].TopicID + 1, strAddTopic);
		}
		if (strAddTopic != "Text") {
			for (let i = 0; i < this.defaultOptionNum; i++) {
				let tempOption = new Option(String.fromCharCode(i + 65));
				let indexInitOptionPush = tempTopic.Options.push(tempOption);
			}
		}
		if (strAddTopic == "Radio" || strAddTopic == "Checkbox") {
			tempTopic.isRequired = true;
		}
		this.formControl.Topics.push(tempTopic);
		setTimeout(this.refreshFormControlList, 20)
	}

	public deleteTopicControl(tempTopicNum: number) {
		this.formControl.Topics.splice(tempTopicNum - 1, 1)
		for (var i = tempTopicNum - 1; i < this.formControl.Topics.length; i++) {
			this.formControl.Topics[i].TopicID -= 1
		}
		setTimeout(this.refreshFormControlList, 20)
	}

	public addOptionControl(tempTopicNum: number, tempOptionNum: string) {
		if (this.formControl.Topics[tempTopicNum - 1].Options.length + 1 <= this.numOptionMax) {
			let tempOption = new Option(String.fromCharCode(tempOptionNum.charCodeAt(0) + 1));
			this.formControl.Topics[tempTopicNum - 1].Options.splice(tempOptionNum.charCodeAt(0) - 64, 0, tempOption)
			for (var i = tempOptionNum.charCodeAt(0) - 63; i < this.formControl.Topics[tempTopicNum - 1].Options.length; i++) {
				this.formControl.Topics[tempTopicNum - 1].Options[i].OptionID
					= String.fromCharCode(this.formControl.Topics[tempTopicNum - 1].Options[i].OptionID.charCodeAt(0) + 1)
			}
		} else {
			this._negAlert.warn(this._negTranslate.get('new.wrong.optionTooMuch'));
		}
	}

	public deleteOptionControl(tempTopicNum: number, tempOptionNum: string) {
		if (this.formControl.Topics[tempTopicNum - 1].Options.length > this.numOptionMin) {
			if (this.formControl.Topics[tempTopicNum - 1].Options.length > this.formControl.Topics[tempTopicNum - 1].Limited) {


				this.formControl.Topics[tempTopicNum - 1].Options.splice(tempOptionNum.charCodeAt(0) - 65, 1)
				for (var i = tempOptionNum.charCodeAt(0) - 65; i < this.formControl.Topics[tempTopicNum - 1].Options.length; i++) {
					this.formControl.Topics[tempTopicNum - 1].Options[i].OptionID
						= String.fromCharCode(this.formControl.Topics[tempTopicNum - 1].Options[i].OptionID.charCodeAt(0) - 1)
				}
			}
			else {
				this._negAlert.warn(this._negTranslate.get('new.wrong.optionLessThanLimit'));
			}
		} else {
			this._negAlert.warn(this._negTranslate.get('new.wrong.optionTooSmall'));
		}
	}

	public refreshFormControlList() {
		var refreshSources = document.getElementsByClassName("formList");
		var dragElement = undefined;
		var before = 0
		for (var i = 0; i < refreshSources.length; i++) {
			refreshSources[i].addEventListener('dragstart', function (ev) {
				dragElement = this;
				before = 0
			}, false);

			refreshSources[i].addEventListener('dragend', function (ev) {
				//ev.target.style.backgroundColor = '#fff';
				ev.preventDefault();
			}, false)

			refreshSources[i].addEventListener('dragenter', function (ev) {
				if (dragElement != this && before != this.id && dragElement != undefined) {
					self.exchange(parseInt(this.id), parseInt(dragElement.id))
					before = this.id
				}
			}, false)
		};
		document.ondragover = function (e) { e.preventDefault(); }
		document.ondrop = function (e) { e.preventDefault(); }
	}

	public exchange(topicID1, topicID2) {
		if (topicID1 <= this.formControl.Topics.length && topicID2 <= this.formControl.Topics.length
			&& topicID1 > 0 && topicID2 > 0) {
			this.formControl.Topics[topicID1 - 1] = this.formControl.Topics.splice(topicID2 - 1, 1, this.formControl.Topics[topicID1 - 1])[0];
			this.formControl.Topics[topicID2 - 1].TopicID = topicID2
			this.formControl.Topics[topicID1 - 1].TopicID = topicID1
		}
	}

	public publishAndSave(isSave) {
		this.formControl.Topics.forEach(topic => {
			topic.Options.forEach(option => {
				option.TopicID = topic.TopicID
			})
		})
		this.formControl.status = (isSave ? 0 : 1)
		var allRight = this.legitimacyJudgment()
		if (allRight) {
			if (this.formControl.Description.length > 2000) {
				this._negAlert.warn(this._negTranslate.get('new.wrong.descriptionTooLong'))
			}
			else {
				if (this.formControl.QuestionnaireID == 0 || this.formControl.QuestionnaireID == undefined) {
					this.questionnaireService.OnPost(this.formControl)
						.then(({ data }) => {
							if (data.Succeeded) {
								this._negStorage.memory.remove("PageSize");
								this._negStorage.memory.remove("PageIndex");
								this._negStorage.memory.remove("Title");
								if (this.formControl.status == 1) {
									this.formControl.QuestionnaireID = data.Questionnaire.QuestionnaireID
									this.pubSuccess = true
									if (data.MailSucceeded) {
										this.sendSuccess = true
									}
									$('#modal-container-268549').modal({ backdrop: 'static' })
								}
								else {
									this.formControl.QuestionnaireID = data.Questionnaire.QuestionnaireID
									this._negAlert.success(this._negTranslate.get('new.transfer.saveRight'))
								}
							}
							else {
								if (this.formControl.status == 1) {
									this._negAlert.success(this._negTranslate.get('new.transfer.publishFailed'))
								}
								else {
									this._negAlert.success(this._negTranslate.get('new.transfer.saveFailed'))
								}
							}
						},
						error => {
							if (this.formControl.status == 1) {
								this._negAlert.success(this._negTranslate.get('new.transfer.publishFailed'))
							}
							else {
								this._negAlert.success(this._negTranslate.get('new.transfer.saveFailed'))
							}
						})
				}
				else {
					this.questionnaireService.OnPut(this.formControl)
						.then(({ data }) => {
							if (data.Succeeded) {
								if (this.formControl.status == 1) {
									this.pubSuccess = true
									if (data.MailSucceeded) {
										this.sendSuccess = true
									}
									$('#modal-container-268549').modal({ backdrop: 'static' })
									this._negStorage.memory.remove("PageSize");
									this._negStorage.memory.remove("PageIndex");
									this._negStorage.memory.remove("Title");
								}
								else {
									this._negAlert.success(this._negTranslate.get('new.transfer.saveRight'))
								}
							}
							else {
								if (this.formControl.status == 1) {
									this._negAlert.success(this._negTranslate.get('new.transfer.publishFailed'))
								}
								else {
									this._negAlert.success(this._negTranslate.get('new.transfer.saveFailed'))
								}
							}
						},
						error => {
							if (this.formControl.status == 1) {
								this._negAlert.success(this._negTranslate.get('new.transfer.publishFailed'))
							}
							else {
								this._negAlert.success(this._negTranslate.get('new.transfer.saveFailed'))
							}
						})
				}
			}
		}
		else {
			this._negAlert.warn(this._negTranslate.get('new.wrong.formatWrong'))
		}
	}

	public reduction(topicID) {
		document.getElementById(topicID).style.display = "none";
	}

	public legitimacyJudgment() {
		var allRight = true
		if (this.formControl.status == 0) {
			return allRight
		}
		if (this.formControl.Title == undefined || this.formControl.Title == null || this.formControl.Title.trim() == "") {
			allRight = false
			document.getElementById("questionnaireWrong").style.display = 'inline'
		}
		if (this.formControl.DueDate == undefined || this.formControl.DueDate == null || this.formControl.DueDate.toString() == "" || this.formControl.DueDate <= new Date()) {
			allRight = false
			document.getElementById("deadlineWrong").style.display = 'inline'
		}
		if (this.formControl.Topics.length == 0) {
			allRight = false
			document.getElementById("topicWrong").style.display = 'inline'
		}
		for (var i = 0; i < this.formControl.Topics.length; i++) {
			if (this.formControl.Topics[i].TopicTitle == null || this.formControl.Topics[i].TopicTitle.trim() == "") {
				allRight = false
				document.getElementById(this.formControl.Topics[i].TopicID + "Wrong").style.display = 'inline'
			}
			for (var j = 0; j < this.formControl.Topics[i].Options.length; j++) {
				if (this.formControl.Topics[i].Options[j].OptionTitle == null || this.formControl.Topics[i].Options[j].OptionTitle.trim() == "") {
					allRight = false
					document.getElementById(this.formControl.Topics[i].TopicID + this.formControl.Topics[i].Options[j].OptionID + "Wrong").style.display = 'inline'
				}
			}
		}
		return allRight
	}

	public setAppearance(isSetView: boolean) {
		this.setAppearanceView = isSetView;
	}

	public priviewTheQuestionnaire() {
		console.log(this.formControl)
		let tempStr = JSON.stringify(this.formControl);
		this._negStorage.memory.set("PreQuestionnaire", [tempStr, "create"]);
		// this._route.navigate(['/eggrolls/preview']);
		this.negMultiTab.openPage('/eggrolls/preview', null, false)
	}

	public limitJudgment(topicID) {
		let a: any = this.formControl.Topics[topicID - 1].Limited.toString()
		a = a.replace(/[０１２３４５６７８９]/g, function (v) { return v.charCodeAt(0) - 65296; });
		let temp = parseInt(a);
		if (isNaN(temp)) {
			this.formControl.Topics[topicID - 1].Limited = 0
			this._negAlert.warn(this._negTranslate.get('new.wrong.format'))
		}
		else if (temp > this.formControl.Topics[topicID - 1].Options.length) {
			this.formControl.Topics[topicID - 1].Limited = 0
			this._negAlert.warn(this._negTranslate.get('new.wrong.greater'))
		}
		else if (temp < 2) {
			this.formControl.Topics[topicID - 1].Limited = 0
			this._negAlert.warn(this._negTranslate.get('new.wrong.smaller'))
			this._negAlert.notify('Notify', () => { }, { type: 'success' });
		}
		else {
			this.formControl.Topics[topicID - 1].Limited = temp
		}
	}

	public getStanderdDate(dateGet) {
		if (dateGet != undefined && dateGet != null) {
			return dateGet.substr(0, 6) == '/Date(' ? this.date(dateGet) : new Date(dateGet);
		}
	}

	public dmft(d) {
		return d.getFullYear() + '-' + this.pad(d.getMonth() + 1) + '-' + this.pad(d.getDate()) + ' ' + this.pad(d.getHours()) + ':' + this.pad(d.getMinutes());
	}

	public date(s) {
		return new Date(parseFloat(/Date\(([^)]+)\)/.exec(s)[1]));
	}

	public pad(d) {
		return d < 10 ? '0' + d : d;
	}

	public titlePlace(input) {
		document.getElementById('questionnaire').placeholder = this._negTranslate.get('new.questionnaire.titleTip')
	}

	public select(id) {
		if (id == this.selectedOption) {
			document.getElementById('img' + this.selectedOption.toString()).style.backgroundColor = 'white'
			this.formControl.BackgroundImageUrl = ""
			this.selectedOption = -1
		}
		else if (this.selectedOption != -1) {
			this.formControl.BackgroundImageUrl = this.imgUrls[id]
			document.getElementById('img' + this.selectedOption.toString()).style.backgroundColor = 'white'
			document.getElementById('img' + id).style.backgroundColor = 'rgb(162, 210, 160)'
			this.selectedOption = id
		}
		else {
			this.formControl.BackgroundImageUrl = this.imgUrls[id]
			document.getElementById('img' + id).style.backgroundColor = 'rgb(162, 210, 160)'
			this.selectedOption = id
		}
	}
	public backToHome() {
		// this._route.navigate(['/eggrolls/index']);
		this.negMultiTab.openPage('/eggrolls/index', null, false)
	}
	public trimString(source: string) {
		return source == undefined ? "" : source.trim()
	}
}
