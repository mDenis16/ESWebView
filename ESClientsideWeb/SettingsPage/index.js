var optionTemplates = {
    checkbox: `
    <div class="name">{OPTION_NAME}</div>
    <div class="input">
        <label class="switch">
            <input accessType='{OPTION_ACCESSTYPE}' class="input__checkbox" displayName='{OPTION_NAME}' type="checkbox" propertyName={PROPERTY_NAME} {OPTION_VALUE}>
            <span class="slider round"></span>
        </label>
    </div>
`,
    textinput: `
<div class="name">{OPTION_NAME}</div>
<div class="input">
    <input accessType='{OPTION_ACCESSTYPE}' type="text" class="input__text" displayName='{OPTION_NAME}' name="{OPTION_NAME}" propertyName={PROPERTY_NAME} value="{OPTION_VALUE}" placeholder="{OPTION_PLACEHOLDER}" {OPTION_URLITEM} {OPTION_DISABLED} >
</div>
`
}
var currentPopulatedSettings = [];

function getTemplateByType(type) {
    switch (type) {
        case 'boolean':
            return optionTemplates.checkbox;
        case 'string':
            return optionTemplates.textinput;
    }
    return undefined;
}
function fillTemplatebyType(setting) {

    var template = getTemplateByType(setting.type);
    var tempTemplate = template.replaceAll('{OPTION_NAME}', setting.displayName);

    if (tempTemplate) {
        if (setting.type == 'boolean')
            tempTemplate = tempTemplate.replaceAll('{OPTION_VALUE}', setting.value ? 'checked' : '');
        else if (setting.type == 'string') {
            tempTemplate = tempTemplate.replaceAll('{OPTION_VALUE}', setting.value);


            if (setting.value_placeholder)
                tempTemplate = tempTemplate.replaceAll('{OPTION_PLACEHOLDER}', setting.value_placeholder);
            else
                tempTemplate = tempTemplate.replaceAll('{OPTION_PLACEHOLDER}', '');
        }
        tempTemplate = tempTemplate.replaceAll('{PROPERTY_NAME}', setting.propertyName);

        if (setting.accessType & 2)
            tempTemplate = tempTemplate.replaceAll('{OPTION_DISABLED}', 'disabled');

        tempTemplate = tempTemplate.replace('{OPTION_ACCESSTYPE}', setting.accessType);

    }

    return tempTemplate;
}
var escapeJSON = function (str) {
    return str.replace('//', '\\');
};
function populateSettings(settings) {

    try {
        settings = JSON.parse(`${settings.replaceAll('\\', '\\\\')}`)
    }
    catch (ex) {
        console.warn(ex);
    }
    var settingsParrent = document.getElementsByClassName('toggle_options')[0];

    if (!settingsParrent) return;

    settingsParrent.innerHTML = '';
    
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
        else if (setting.type == 'boolean') {
            setting.value = option.children[1].children[0].children[0].checked;
            console.log('checked ' + option.children[1].children[0].children[0].checked);
        }
        setting.ref = null;
    });

    var json = JSON.stringify(currentPopulatedSettings);

    console.log('currentSettings ' + json);

    window.chrome.webview.postMessage({ Type: 0, PayLoad: json });
}

document.addEventListener('click', function (ev) {
    if (ev.target && ev.target.classList.contains('input__checkbox') && ev.target.hasAttribute('propertyName')) {

        var res = currentPopulatedSettings.find(p => p.propertyName == ev.target.getAttribute('propertyName'));

        if (res) {
            res.value = !res.value;
        }
    }
});

document.addEventListener('focusin', (ev) => {

    if (ev.target && ev.target.classList.contains('input__text') && ev.target.hasAttribute('propertyName')) {

        if (ev.target.getAttribute('propertyName') == 'loadURL') {

            var options = document.getElementsByClassName('toggle_options')[0];
            if (!options) return;
            var disabled_list = document.querySelectorAll('[accessType]');
            if (!disabled_list) return;


            disabled_list.forEach(el => {
                if (!(parseInt(el.getAttribute("accessType")) & 4)) return;

                el.setAttribute('oldValue', el.value);

                el.value = `{${el.getAttribute('propertyName')}}`
            })

        }
    }
});
document.addEventListener('focusout', (ev) => {

    if (ev.target && ev.target.classList.contains('input__text') && ev.target.hasAttribute('propertyName')) {

        if (ev.target.getAttribute('propertyName') == 'loadURL') {

            var options = document.getElementsByClassName('toggle_options')[0];
            if (!options) return;
            var disabled_list = document.querySelectorAll('[accessType]');
            if (!disabled_list) return;




            disabled_list.forEach(el => {
                if (parseInt(el.getAttribute('accessType')) & 4)
                    el.value = el.getAttribute("oldValue");
            })

        }
    }
});
