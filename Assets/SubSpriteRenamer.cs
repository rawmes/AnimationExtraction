using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SubSpriteRenamer : MonoBehaviour
{
    [Header("Sprite Rename Variables")]
    public Texture2D texture2D;
    public string newName;

    [Header("Animation Rename Variables")]
    public AnimationClip[] animationClips;
    public string findString = "character000";
    public string replaceString = "character001";

    [Header("Animation Controller Variables")]
    public Animator animator;
    [SerializeField] private bool changeAssetName = true; // Whether to also change the asset name
    [SerializeField] private bool verboseLogging = true; // Enable verbose logging

    [ContextMenu("Rename Sprite")]
    void RenameSprite()
    {
        string path = AssetDatabase.GetAssetPath(texture2D);
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texture2D);
        dataProvider.InitSpriteEditorDataProvider();

        var spriteRects = dataProvider.GetSpriteRects();
        for (int i = 0; i < spriteRects.Length; ++i)
        {
            spriteRects[i].name = string.Format(newName + "_{0}", i);
            Debug.Log(spriteRects[i].name);
        }

        dataProvider.SetSpriteRects(spriteRects);
        dataProvider.Apply();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
    [ContextMenu("Rename Animations")]
    void RenameAnimations()
    {

        foreach (var clip in animationClips)
        {
            // Get the original animation clip name
            string originalName = clip.name;

            // Replace the specified substring
            string newName = originalName.Replace(findString, replaceString);

            // Update the animation clip name
            clip.name = newName;

            // Rename the corresponding asset
            string assetPath = AssetDatabase.GetAssetPath(clip);
            AssetDatabase.RenameAsset(assetPath, newName);

            // Print the new animation clip name
            Debug.Log("Renamed animation: " + clip.name);
        }

    }

    [ContextMenu("Fix Animation Controller")]
    void FixAnimController()
    {
        // Update animation clip references
        if (animator == null)
        {
            Debug.LogError("Animator reference is missing. Attach an Animator component to this GameObject.");
            return;
        }

        // Get the Animator Controller
        var controller = animator.runtimeAnimatorController as RuntimeAnimatorController;
        if (controller == null)
        {
            Debug.LogError("Animator does not have an AnimatorController assigned.");
            return;
        }

        // Iterate through all layers in the Animator Controller
        foreach (var clip in controller.animationClips)
        {
            string originalName = clip.name;



            // Print the new animation clip name
            if (verboseLogging)
                Debug.Log($"Renamed animation clip: {clip.name}");

            // Optionally, update the asset name
            if (changeAssetName)
            {
                string assetPath = AssetDatabase.GetAssetPath(clip);
                string newPath = assetPath.Replace(originalName, clip.name);
                AssetDatabase.RenameAsset(assetPath, clip.name);

                if (verboseLogging)
                    Debug.Log($"Renamed asset: {assetPath} to {newPath}");
            }


        }

        // Save the changes to the Animator Controller
        AssetDatabase.SaveAssets();
    }
}






