var optionTemplates = {
    checkbox: `<div class="option">
    <div class="name">{OPTION_NAME}</div>
    <div class="input">
        <label class="switch">
            <input class="input__checkbox" type="checkbox" {OPTION_VALUE}>
            <span class="slider round"></span>
        </label>
    </div>
</div>`,
    textinput: `<div class="option">
<div class="name">{OPTION_NAME}</div>
<div class="input">
    <input type="text" class="input__text" name="{OPTION_NAME}" value="{OPTION_VALUE}" placeholder="{OPTION_PLACEHOLDER}" >
</div>
</div>`
}
function getTemplateByType(type) {
    switch (type) {
        case 'bool':
            return optionTemplates.checkbox;
        case 'text':
            return optionTemplates.textinput;
    }
    return undefined;
}
function fillTemplatebyType(setting) {
    var template = getTemplateByType(setting.type);

    var tempTemplate = template.replace('{OPTION_NAME}', setting.name);
    
    if (tempTemplate) {
        if (setting.type == 'bool')
            tempTemplate = tempTemplate.replace('{OPTION_VALUE}', setting.value ? 'checked' : '');
        else if (setting.type == 'text') {
            tempTemplate = tempTemplate.replace('{OPTION_VALUE}', setting.value);

          
            if (setting.value_placeholder)
                tempTemplate = tempTemplate.replace('{OPTION_PLACEHOLDER}', setting.value_placeholder);
            else
                tempTemplate = tempTemplate.replace('{OPTION_PLACEHOLDER}', '');
        }
    }
    
    return tempTemplate;
}
function populateSettings(settings) {

    var settingsParrent = document.getElementsByClassName('toggle_options')[0];

    if (!settingsParrent) return;

    settings.forEach(setting => {

        var template = fillTemplatebyType(setting);
        if (!template) return;

        let doc = document.createElement('div');
        console.log('created doc ' + template);
        doc.innerHTML = template;
        settingsParrent.appendChild(doc);

    });
}