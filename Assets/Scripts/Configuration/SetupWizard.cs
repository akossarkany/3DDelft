using Netherlands3D.Core;
using Netherlands3D.Twin.Features;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Netherlands3D.Twin.Configuration
{
    public class SetupWizard : MonoBehaviour
    {
        [SerializeField] private Configuration configuration;

        [Header("References")] 
        [SerializeField] private TMP_InputField titleField;
        [SerializeField] private TMP_InputField originXField;
        [SerializeField] private TMP_InputField originYField;
        [SerializeField] private TMP_InputField shareUrlField;

        [SerializeField] private GameObject featureList;

        [Header("Prefab")] [SerializeField] private Toggle featureSelectionPrefab;

        private void Start()
        {
            titleField.text = configuration.Title;
            titleField.onValueChanged.AddListener(value => configuration.Title = value);
            
            originXField.text = configuration.Origin.x.ToString();
            originXField.onValueChanged.AddListener(OnOriginXChanged);

            originYField.text = configuration.Origin.y.ToString();
            originYField.onValueChanged.AddListener(OnOriginYChanged);

            foreach (var availableFeature in configuration.Features)
            {
                Toggle featureSelection = Instantiate(featureSelectionPrefab, featureList.transform);
                featureSelection.isOn = availableFeature.IsEnabled;
                featureSelection.onValueChanged.AddListener(value => OnFeatureChanged(availableFeature, value));
                featureSelection.GetComponentInChildren<Text>().text = availableFeature.Caption;
            }
        }

        /// <summary>
        /// Assign listeners to the ScriptableObject in onEnable because they need to be explicitly removed
        /// in OnDisable to prevent lingering listeners on the scriptable object as soon as this object is destroyed.
        /// </summary>
        private void OnEnable()
        {
            configuration.OnOriginChanged.AddListener(ValidateRdCoordinates);
            configuration.OnOriginChanged.AddListener(UpdateShareUrlWhenOriginChanges);
            configuration.OnTitleChanged.AddListener(UpdateShareUrlWhenTitleChanges);
            foreach (var availableFeature in configuration.Features)
            {
                availableFeature.OnEnable.AddListener(UpdateShareUrlWhenFeatureChanges);
                availableFeature.OnDisable.AddListener(UpdateShareUrlWhenFeatureChanges);
            }
        }

        private void ValidateRdCoordinates(Vector3RD coordinates)
        {
            var validRdCoordinates = CoordConvert.RDIsValid(coordinates);
            originXField.textComponent.color = validRdCoordinates ? Color.black : Color.red;
            originYField.textComponent.color = validRdCoordinates ? Color.black : Color.red;
        }

        /// <summary>
        /// Explicitly remove listeners on the ScriptableObject to prevent lingering listeners on the scriptable object
        /// as soon as this object is destroyed.
        /// </summary>
        private void OnDisable()
        {
            configuration.OnOriginChanged.RemoveListener(ValidateRdCoordinates);
            configuration.OnOriginChanged.RemoveListener(UpdateShareUrlWhenOriginChanges);
            configuration.OnTitleChanged.RemoveListener(UpdateShareUrlWhenTitleChanges);
            foreach (var availableFeature in configuration.Features)
            {
                availableFeature.OnEnable.RemoveListener(UpdateShareUrlWhenFeatureChanges);
                availableFeature.OnDisable.RemoveListener(UpdateShareUrlWhenFeatureChanges);
            }
        }

        private void UpdateShareUrlWhenFeatureChanges()
        {
            shareUrlField.text = configuration.GenerateQueryString();
        }

        private void UpdateShareUrlWhenOriginChanges(Vector3RD origin)
        {
            shareUrlField.text = configuration.GenerateQueryString();
        }

        private void UpdateShareUrlWhenTitleChanges(string arg0)
        {
            shareUrlField.text = configuration.GenerateQueryString();
        }

        private void OnFeatureChanged(Feature availableFeature, bool value)
        {
            availableFeature.IsEnabled = value;
        }

        private void OnOriginYChanged(string value)
        {
            int.TryParse(value, out int y);
            configuration.Origin = new Vector3RD(configuration.Origin.x, y, 300);
        }

        private void OnOriginXChanged(string value)
        {
            int.TryParse(value, out int x);
            configuration.Origin = new Vector3RD(x, configuration.Origin.y, 300);
        }
    }
}