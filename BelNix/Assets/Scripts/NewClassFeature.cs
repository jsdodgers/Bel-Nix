using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewClassFeature : MonoBehaviour {
    //[SerializeField] private GameObject descriptionPanel;
    //[SerializeField] private GameObject namePanel;
    [SerializeField] private BaseManager baseManager;
    [SerializeField] private Text descriptionField, nameField;
    [SerializeField] private Button confirmButton, backButton, featureButtonA, featureButtonB;

    private ClassFeature[] features;
    private ClassFeature currentFeature;
    private int selectedFeature = 0;
    private const string defaultName = "<b>CHOOSE A CLASS FEATURE</b>";
    private const string defaultDescription = "You have gained a level! Choose a new class feature by clicking on a button above.";

	// Use this for initialization
	void Start () {
        reset();
	}

    private void reset()
    {
        featureButtonA.gameObject.SetActive(false);
        featureButtonB.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        features = new ClassFeature[2];
    }

    public void format(ClassFeature feature)
    {
        reset();
        features[0] = feature;
        setDescription(0);
        currentFeature = feature;
    }

    public void format(ClassFeature featureA, ClassFeature featureB)
    {
        featureButtonA.gameObject.SetActive(true);
        featureButtonB.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
        features[0] = featureA;
        features[1] = featureB;
        featureButtonA.transform.GetChild(0).GetComponent<Text>().text = ClassFeatures.getName(featureA);
        featureButtonB.transform.GetChild(0).GetComponent<Text>().text = ClassFeatures.getName(featureB);
    }

    public void setDescription(int featureOption)
    {
        descriptionField.text = ClassFeatures.getDescription(features[featureOption]);
        nameField.text = ClassFeatures.getName(features[featureOption]);
        currentFeature = features[featureOption];
        selectedFeature = featureOption;
        if (features.Length > 1)
            confirmButton.interactable = true;
    }

    public void hover(int featureOption)
    {
        if (currentFeature != null)
            return;
        descriptionField.text = ClassFeatures.getDescription(features[featureOption]);
        nameField.text = ClassFeatures.getName(features[featureOption]);
    }

    public void dismiss()
    {
        reset();
        baseManager.disableNewClassFeaturePrompt(currentFeature, selectedFeature);
    }
}
