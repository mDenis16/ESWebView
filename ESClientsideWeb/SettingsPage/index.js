var optionTemplates = {
    checkbox: `
    <div class="name">{OPTION_NAME}</div>
    <div class="input">
        <label class="switch">
            <input class="input__checkbox" type="checkbox" propertyName={PROPERTY_NAME} {OPTION_VALUE}>
            <span class="slider round"></span>
        </label>
    </div>
`,
    textinput: `
<div class="name">{OPTION_NAME}</div>
<div class="input">
    <input type="text" class="input__text" name="{OPTION_NAME}" propertyName={PROPERTY_NAME} value="{OPTION_VALUE}" placeholder="{OPTION_PLACEHOLDER}" >
</div>
`
}
var currentPopulatedSettings = [];

function getTemplateByType(type) {
    switch (type) {
        case 'bool':
            return optionTemplates.checkbox;
        case 'string':
            return optionTemplates.textinput;
    }
    return undefined;
}
function fillTemplatebyType(setting) {
    var template = getTemplateByType(setting.type);
    var tempTemplate = template.replace('{OPTION_NAME}', setting.displayName);

    if (tempTemplate) {
        if (setting.type == 'bool')
            tempTemplate = tempTemplate.replace('{OPTION_VALUE}', setting.value ? 'checked' : '');
        else if (setting.type == 'string') {
            tempTemplate = tempTemplate.replace('{OPTION_VALUE}', setting.value);


            if (setting.value_placeholder)
                tempTemplate = tempTemplate.replace('{OPTION_PLACEHOLDER}', setting.value_placeholder);
            else
                tempTemplate = tempTemplate.replace('{OPTION_PLACEHOLDER}', '');
        }
        tempTemplate = tempTemplate.replace('{PROPERTY_NAME}', setting.propertyName);
    }

    return tempTemplate;
}
function populateSettings(settings) {
    settings = JSON.parse(settings);

    var settingsParrent = document.getElementsByClassName('toggle_options')[0];
    
    if (!settingsParrent) return;

    settings.forEach(setting => {
        
        var template = fillTemplatebyType(setting);
    
        if (!template) return;

        let doc = document.createElement('div');
        doc.className = 'option';
       
        doc.innerHTML = template;
        settingsParrent.appendChild(doc);
        setting.ref = doc;
    });

    currentPopulatedSettings = settings;
}


function cancelCommand() {

}
function saveCommand() {
    currentPopulatedSettings.forEach(setting => {
        var option = setting.ref;
        if (!option) return;



        if (setting.type == 'string')
            setting.value = option.children[1].children[0].value;
        else if (setting.type == 'bool') {
            setting.value = option.children[1].children[0].children[0].checked;
            console.log('checked ' + option.children[1].children[0].children[0].checked);
        }
        setting.ref = null;
    });

    var json = JSON.stringify(currentPopulatedSettings);

    console.log('currentSettings ' + json);

    window.chrome.webview.postMessage({Type: 0, PayLoad: json});
}

document.addEventListener('click', function (ev) {
    if (ev.target && ev.target.classList.contains('input__checkbox') && ev.target.hasAttribute('propertyName')) {

        var res = currentPopulatedSettings.find(p => p.propertyName == ev.target.getAttribute('propertyName'));

        if (res) {
            res.value = !res.value;
        }
    }
});

