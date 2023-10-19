using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Netherlands3D.Twin.UI
{
    public class Toolbar : MonoBehaviour
    {
        [SerializeField] private string toolbarName = "toolbar";
        [SerializeField] private List<Tool> tools = new();
        [SerializeField] private VisualTreeAsset toolButtonAsset;
        public UnityEvent<Tool> onActivateTool = new();
        public UnityEvent<Tool> onDeactivateTool = new();

        private RadioButton activeSelection = null;

        private void OnEnable()
        {
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
            var toolbar = rootVisualElement.Q(name: toolbarName);
            foreach (var tool in tools)
            {
                var toolButton = toolButtonAsset.Instantiate();
                toolButton.AddToClassList("toolbar__button--" + tool.icon);
                toolButton.viewDataKey = tool.code;
                var toolButtonRadioButton = toolButton.Q<RadioButton>();
                toolButtonRadioButton.RegisterCallback<ClickEvent>(ToggleRadioButton);
                toolButtonRadioButton.RegisterValueChangedCallback(OnRadioButtonChanged);
                toolbar.Q<GroupBox>().Add(toolButton);
            }
        }

        private void ToggleRadioButton(ClickEvent evt)
        {
            ToggleRadioButton(evt.target as RadioButton);
        }

        private void ToggleRadioButton(RadioButton clickedRadioButton)
        {
            activeSelection = activeSelection == clickedRadioButton ? null : clickedRadioButton;
            
            // Uncheck the radiobutton if we click on the same one a second time
            if (activeSelection == null)
            {
                clickedRadioButton.value = false;
            }
        }

        private void OnRadioButtonChanged(ChangeEvent<bool> evt)
        {
            var radioButton = evt.target as RadioButton;
            if (radioButton == null) return;
            if (evt.previousValue == evt.newValue) return;

            var tool = ExtractToolFromRadioButton(radioButton);

            radioButton.EnableInClassList("toolbar__button--active", evt.newValue);
            
            switch (evt.newValue)
            {
                case true: 
                    onActivateTool.Invoke(tool);
                    tool.Activate(); 
                    break;
                default:
                    onDeactivateTool.Invoke(tool);
                    tool.Deactivate();
                    break;
            }
        }

        private Tool ExtractToolFromRadioButton(RadioButton radioButton)
        {
            // If it is a template, the parent may contain the correct view data key
            var toolIdentifier = string.IsNullOrEmpty(radioButton.viewDataKey) == false
                ? radioButton.viewDataKey
                : radioButton.parent.viewDataKey;

            if (string.IsNullOrEmpty(toolIdentifier))
            {
                Debug.LogError(
                    "The tool button you just toggled does not have its ViewDataKey set, we expect a code that "
                    + "signals which tool is associated with this button"
                );
            }

            return tools.Find(tool => string.Equals(tool.code, toolIdentifier));
        }
    }
}