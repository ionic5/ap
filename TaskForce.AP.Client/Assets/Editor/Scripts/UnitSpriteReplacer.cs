#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TaskForce.AP.Client.Editor
{
    public class UnitSpriteReplacer
    {
        [MenuItem("Assets/Replace CreatedObject Sprites by Number", false, 10)]
        private static void ReplaceSpritesInSelectedFolder()
        {
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Select folder");
                return;
            }

            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
            if (textureGuids.Length == 0)
            {
                Debug.LogError($"Failed to find sprite sheet in {folderPath}");
                return;
            }

            string newSheetPath = AssetDatabase.GUIDToAssetPath(textureGuids[0]);
            Sprite[] newSpritesArray = AssetDatabase.LoadAllAssetsAtPath(newSheetPath)
                .OfType<Sprite>()
                .ToArray();

            if (newSpritesArray.Length == 0)
            {
                Debug.LogError("Failed to find sprite in sprite sheet. Check slice in sprite sheet.");
                return;
            }

            Dictionary<int, Sprite> newSpritesMap = new Dictionary<int, Sprite>();
            Regex numberRegex = new Regex(@"(\d+)$");

            foreach (var sprite in newSpritesArray)
            {
                Match match = numberRegex.Match(sprite.name);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int number))
                {
                    newSpritesMap[number] = sprite;
                }
            }

            if (newSpritesMap.Count == 0)
            {
                Debug.LogError("Failed to find sprite sequence number in new sprite sheet.");
                return;
            }

            string[] clipGuids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });

            Debug.Log($"[START] Processing {clipGuids.Length} total clips. Mappable new sprites found: {newSpritesMap.Count}");

            string newFolderPath = Path.Combine(folderPath, "ReplacedClips");
            if (!AssetDatabase.IsValidFolder(newFolderPath))
            {
                AssetDatabase.CreateFolder(folderPath, "ReplacedClips");
            }

            foreach (string clipGuid in clipGuids)
            {
                string originalClipPath = AssetDatabase.GUIDToAssetPath(clipGuid);
                AnimationClip originalClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(originalClipPath);

                string newClipPath = Path.Combine(newFolderPath, Path.GetFileName(originalClipPath));
                AssetDatabase.CopyAsset(originalClipPath, newClipPath);
                AnimationClip newClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(newClipPath);

                EditorCurveBinding binding = new EditorCurveBinding
                {
                    path = "",
                    type = typeof(SpriteRenderer),
                    propertyName = "m_Sprite"
                };

                ObjectReferenceKeyframe[] originalKeyframes = AnimationUtility.GetObjectReferenceCurve(originalClip, binding);

                if (originalKeyframes.Length == 0) continue;

                ObjectReferenceKeyframe[] newKeyframes = new ObjectReferenceKeyframe[originalKeyframes.Length];
                bool success = true;

                for (int i = 0; i < originalKeyframes.Length; i++)
                {
                    Sprite originalSprite = originalKeyframes[i].value as Sprite;
                    if (originalSprite == null) continue;

                    Match originalMatch = numberRegex.Match(originalSprite.name);

                    if (originalMatch.Success && int.TryParse(originalMatch.Groups[1].Value, out int numberToFind))
                    {
                        if (newSpritesMap.TryGetValue(numberToFind, out Sprite replacementSprite))
                        {
                            newKeyframes[i] = new ObjectReferenceKeyframe
                            {
                                time = originalKeyframes[i].time,
                                value = replacementSprite
                            };
                        }
                        else
                        {
                            Debug.LogError($"Error processing clip '{originalClip.name}': Could not find a new sprite corresponding to number {numberToFind}."); success = false;
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Error processing clip '{originalClip.name}': Could not find a sequence number in the original sprite name '{originalSprite.name}'."); success = false;
                        break;
                    }
                }

                if (success)
                {
                    AnimationUtility.SetObjectReferenceCurve(newClip, binding, newKeyframes);
                    Debug.Log($"Clip '{originalClip.name}' → '{newClip.name}' Successfuly replaced. (See ReplacedClips folder).");
                }
                else
                {
                    AssetDatabase.DeleteAsset(newClipPath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Completed] Replace unit sprite has completed.");
        }

        [MenuItem("Assets/Replace CreatedObject Sprites by Number", true)]
        private static bool ValidateReplaceSprites()
        {
            return Selection.activeObject != null && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));
        }
    }
}
#endif