function addTextInputToElement(parentElementId, inputClasses) {
    const element = document.getElementById(parentElementId);

    const input = document.createElement('input');
    input.type = 'text';
    input.className = inputClasses;

    element.appendChild(input);
}

function buildJsonFromInputs(inputClass) {
    const inputs = document.getElementsByClassName(inputClass);
    const values = [];

    for (let i = 0; i < inputs.length; i++) {
        const input = inputs[i];
        values.push(input.value);
    }

    return JSON.stringify(values);
}

// Example usage:
// addTextInputToClass('target-class');
// const jsonString = buildJsonFromInputs('target-class');
// console.log(jsonString);
