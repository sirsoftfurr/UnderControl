using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RedstoneinventeGameStudio.Utilities.AnimationRebinder
{
    [InitializeOnLoad]
    public class AnimationRebinderWindow : EditorWindow
    {
        static GUIStyle baseBoxStyle;
        static GUIStyle overlayBoxStyle;

        static GUIStyle objectFieldStyle;
        static GUIStyle dropdownStyle;

        static GUIStyle headerStyle;
        static GUIStyle foldoutStyle;
        static GUIStyle foldoutItemStyle;

        static GUIStyle subHeaderStyle;
        static GUIStyle buttonStyle;

        private GUIStyle sliderStyle;
        private GUIStyle thumbStyle;
        private Color backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        private Color thumbColor = new Color(0.4f, 0.8f, 0.4f);
        private Color sliderColor = new Color(0.2f, 0.6f, 0.8f);

        private AnimationClip animationClip;
        AnimationClip duplicate;
        GameObject template;

        static bool partialMatch;

#nullable enable
        AnimTree? animTree;
#nullable disable

        bool ovveriteCurrent;

        Dictionary<string, string> changes = new Dictionary<string, string>();

        [MenuItem("Tools/RebindAnimator")]
        public static void ShowWindow()
        {
            GetWindow<AnimationRebinderWindow>("RebindAnimator");
        }

        private void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        private void OnAfterAssemblyReload()
        {
            animTree = new AnimTree();
            changes = new Dictionary<string, string>();
        }

        void OnFocus()
        {
            try
            {
                partialMatch = false;
                animTree.root.ResetTemplate();


                if (template != default)
                {
                    animTree.root.AddTemplateTest(template);
                }
                //else
                //{

                //}
            }
            catch (Exception ex)
            {

            }
        }

        private void OnGUI()
        {
            if (animTree == default)
            {
                animTree = new AnimTree();
            }

            Style();

            EditorGUILayout.BeginVertical(baseBoxStyle);

            GUILayout.Label("Anim Rebinder", EditorStyles.boldLabel);

            //AnimationClip clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), false);
            //GameObject go = (GameObject)EditorGUILayout.ObjectField("Template Object", template, typeof(GameObject), true);

            AnimationClip clip = DrawObjectField("Animation Clip", animationClip);
            GameObject go = DrawObjectField("Template Object", template);

            //DrawObjectField("Animation Clip", clip);
            //DrawObjectField("Template Object", go);

            if (go == default || go != template)
            {
                animTree.root.ResetTemplate();
                template = go;
            }

            if (template != default)
            {
                animTree.root.AddTemplateTest(template);
            }

            AnimNode.noTemplate = template == default;

            if (clip != animationClip)
            {
                animTree = new AnimTree();
                changes = new Dictionary<string, string>();
            }

            animationClip = clip;

            List<string> namesFound = new List<string>();

            if (animationClip == null)
            {
                EditorGUILayout.HelpBox("No animation clips selected!", MessageType.Error);
                EditorGUILayout.EndVertical();
                return;
            }

            DrawCustomBooleanField("Ovveride Current Clip", ref ovveriteCurrent);

            EditorGUILayout.Space();

            foreach (var item in AnimationUtility.GetCurveBindings(animationClip))
            {
                if (namesFound.Contains(item.path))
                {
                    continue;
                }

                //string newPath = DrawRebindPath(item.path);

                //if (changes.ContainsKey(item.path))
                //{
                //    if (newPath == item.path)
                //    {
                //        changes.Remove(item.path);
                //    }
                //    else
                //    {
                //        changes[item.path] = newPath;
                //    }
                //}
                //else
                //{
                //    if (newPath != item.path)
                //    {
                //        changes.Add(item.path, newPath);
                //    }
                //}

                namesFound.Add(item.path);

                animTree.AddNode(item.path);
            }

            animTree.DrawTree();

            EditorGUILayout.Space();

            if (partialMatch)
            {
                EditorGUILayout.HelpBox("Not all animation nodes could be matched! Re-adjust object hierarchy if needed!", MessageType.Warning);
            }

            if (template != default)
            {
                if (GUILayout.Button("Use Template", buttonStyle))
                {
                    ShowUpgradePopup();
                }
            }

            if (GUILayout.Button("Rebind Animation", buttonStyle))
            {
                duplicate = Instantiate(animationClip);

                if (ovveriteCurrent)
                {
                    var pathT = AssetDatabase.GetAssetPath(clip);
                    var newPathT = pathT.Replace(".anim", "_Backup.anim");
                    AssetDatabase.CreateAsset(duplicate, newPathT);
                    AssetDatabase.SaveAssets();

                    duplicate = animationClip;
                }

                //foreach (var change in changes)
                //{
                //    RebindAnimationNoSave(duplicate, change.Key, change.Value);
                //}

                StringPair pair = animTree.GetPaths();

                for (int i = 0; i < pair.original.Count; i++)
                {
                    Debug.Log($"{pair.original[i]} > {pair.changed[i]}");
                    if (i >= pair.changed.Count)
                    {
                        break;
                    }

                    if (pair.original[i] != pair.changed[i])
                    {
                        RebindAnimationNoSave(duplicate, pair.original[i], pair.changed[i]);
                    }
                }

                if (!ovveriteCurrent)
                {
                    var path = AssetDatabase.GetAssetPath(clip);
                    var newPath = ovveriteCurrent ? path : path.Replace(".anim", "_Rebound.anim");
                    AssetDatabase.CreateAsset(duplicate, newPath);
                }

                AssetDatabase.SaveAssets();

                Debug.Log($"Animation rebinding complete.");
                changes = new Dictionary<string, string>();
                animTree = new AnimTree();
            }

            EditorGUILayout.EndVertical();
        }

        void DrawObjectField(string label, ref GameObject selected)
        {
            GUILayout.BeginHorizontal(objectFieldStyle);
            GUILayout.Label(label);

            selected = (GameObject)(EditorGUILayout.ObjectField("", selected, typeof(GameObject), true));

            GUILayout.EndHorizontal();
        }

        void DrawObjectField(string label, ref AnimationClip selected)
        {
            GUILayout.BeginHorizontal(objectFieldStyle);
            GUILayout.Label(label);

            selected = (AnimationClip)(EditorGUILayout.ObjectField("", selected, typeof(AnimationClip), false));

            GUILayout.EndHorizontal();
        }

        GameObject DrawObjectField(string label, GameObject selected)
        {
            GUILayout.BeginHorizontal(objectFieldStyle);
            GUILayout.Label(label);

            selected = (GameObject)(EditorGUILayout.ObjectField("", selected, typeof(GameObject), true));

            GUILayout.EndHorizontal();

            return selected;
        }

        AnimationClip DrawObjectField(string label, AnimationClip selected)
        {
            GUILayout.BeginHorizontal(objectFieldStyle);
            GUILayout.Label(label);

            selected = (AnimationClip)(EditorGUILayout.ObjectField("", selected, typeof(AnimationClip), false));

            GUILayout.EndHorizontal();

            return selected;
        }

        private void ShowUpgradePopup()
        {
            bool confirm = false;

            int dialogResult = EditorUtility.DisplayDialogComplex(
                "Template Hierarchy Validation",
                "Before proceeding, ensure that the template object's hierarchy matches the structure referenced in the animations. All child indices in the template should align exactly with those in the animation. Mismatched hierarchies may cause animations to fail. Please verify and confirm to continue.",
                "Continue",
                "Cancel",
                ""
            );

            switch (dialogResult)
            {
                case 0:
                    confirm = true;
                    break;
                case 1:
                    return;
            }

            if (confirm)
            {
                animTree.SetTemplate(template);
                template = default;
            }
        }

        private void DrawCustomBooleanField(string label, ref bool value)
        {
            EditorGUILayout.BeginHorizontal("Box");

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };

            GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                padding = new RectOffset(10, 10, 5, 5),
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };

            GUILayout.Label(label, labelStyle);

            value = EditorGUILayout.Toggle(value, toggleStyle);

            EditorGUILayout.EndHorizontal();
        }

        private void RebindAnimationNoSave(AnimationClip newClip, string oldBinding, string newBinding)
        {
            if (newClip == null || string.IsNullOrEmpty(oldBinding) || string.IsNullOrEmpty(newBinding))
            {
                Debug.LogWarning("Invalid input. Please provide valid paths.");
                return;
            }

            // Iterate through all bindings and update the paths
            var bindings = AnimationUtility.GetCurveBindings(newClip);
            foreach (var binding in bindings)
            {
                if (binding.path == oldBinding)
                {
                    var curve = AnimationUtility.GetEditorCurve(newClip, binding);
                    var newBindings = binding;
                    newBindings.path = newBinding; // Update the path

                    AnimationUtility.SetEditorCurve(newClip, newBindings, curve);
                    AnimationUtility.SetEditorCurve(newClip, binding, null); // Remove the old curve
                }
            }
        }

        private void RebindAnimation(AnimationClip clip, string oldBinding, string newBinding)
        {
            if (clip == null || string.IsNullOrEmpty(oldBinding) || string.IsNullOrEmpty(newBinding))
            {
                Debug.LogWarning("Invalid input. Please provide valid paths.");
                return;
            }

            // Duplicate the animation clip to avoid modifying the original
            var newClip = Instantiate(clip);

            // Iterate through all bindings and update the paths
            var bindings = AnimationUtility.GetCurveBindings(newClip);
            foreach (var binding in bindings)
            {
                if (binding.path == oldBinding)
                {
                    var curve = AnimationUtility.GetEditorCurve(newClip, binding);
                    var newBindings = binding;
                    newBindings.path = newBinding; // Update the path

                    AnimationUtility.SetEditorCurve(newClip, newBindings, curve);
                    AnimationUtility.SetEditorCurve(newClip, binding, null); // Remove the old curve
                }
            }

            // Save the new clip
            var path = AssetDatabase.GetAssetPath(clip);
            var newPath = path.Replace(".anim", "_Rebound.anim");
            AssetDatabase.CreateAsset(newClip, newPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"Animation rebinding complete. Saved as {newPath}");
        }

        string DrawRebindPath(string oldPath)
        {
            int count = oldPath.Split("/").Length;

            if (count == 1)
            {
                EditorGUILayout.BeginVertical(overlayBoxStyle);

                EditorGUILayout.LabelField($"Rebind {oldPath}");
                string pathToRet = EditorGUILayout.TextField("New Path", changes.ContainsKey(oldPath) ? changes[oldPath] : "");

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                return pathToRet;
            }

            string thisName = oldPath.Split("/")[count - 1];

            EditorGUILayout.BeginVertical(overlayBoxStyle);

            EditorGUILayout.LabelField($"Rebind {thisName}");

            string lastValue = oldPath.Remove(oldPath.LastIndexOf("/"));
            string path = "";

            foreach (var item in lastValue.Split("/"))
            {
                string temp = changes.ContainsKey(item) ? changes[item] : item;
                temp = temp.Length <= 0 ? item : temp;

                path += $"{temp}/";
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(path);

            string newPath = EditorGUILayout.TextField("", changes.ContainsKey(oldPath) ? changes[oldPath] : "");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            return path + newPath;
        }

        //string DrawRebindPath(string oldPath)
        //{
        //    string pathToShow = oldPath;
        //    pathToShow = "";

        //    foreach (var item in oldPath.Split("/"))
        //    {
        //        if (changes.ContainsKey(item))
        //        {
        //            if (changes[item] == "" || changes[item] == oldPath)
        //            {
        //                pathToShow += $"{item}/";
        //                continue;
        //            }

        //            pathToShow += $"{changes[item]}/";
        //        }
        //        else
        //        {
        //            pathToShow += $"{item}/";
        //        }
        //    }

        //    pathToShow = pathToShow.Remove(pathToShow.Length - 1);

        //    EditorGUILayout.BeginVertical(overlayBoxStyle);

        //    EditorGUILayout.LabelField($"Rebind {pathToShow}");
        //    string newPath = EditorGUILayout.TextField("New Path", changes.ContainsKey(oldPath) ? changes[oldPath] : "");

        //    EditorGUILayout.EndVertical();
        //    EditorGUILayout.Space();

        //    return newPath;
        //}

        void Style()
        {
            baseBoxStyle = new GUIStyle(GUI.skin.box);
            baseBoxStyle.normal.background = MakeTexture(1, 1, new Color(0.8f, 0.8f, 0.8f));

            baseBoxStyle.padding = new RectOffset(20, 20, 20, 20);
            baseBoxStyle.margin = new RectOffset(10, 10, 10, 10);

            overlayBoxStyle = new GUIStyle(GUI.skin.box);
            overlayBoxStyle.normal.background = MakeTexture(1, 1, new Color(1f, 1f, 1f, 0.9f));

            overlayBoxStyle.padding = new RectOffset(15, 15, 15, 15);
            overlayBoxStyle.margin = new RectOffset(5, 5, 5, 5);

            headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.1f, 0.5f, 0.8f) }
            };

            subHeaderStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                onNormal = { textColor = Color.white },
                border = new RectOffset(4, 4, 4, 4),
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5),
            };

            objectFieldStyle = new GUIStyle(EditorStyles.objectField)
            {
                margin = new RectOffset(5, 5, 5, 5)
            };

            dropdownStyle = new GUIStyle(EditorStyles.popup)
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(5, 5, 5, 5)
            };

            sliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
            {
                fixedHeight = 10,
            };

            thumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb)
            {
                fixedHeight = 20,
                fixedWidth = 20,
            };

            foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.8f, 0.8f, 0.8f) },
                onNormal = { textColor = new Color(0.5f, 1.0f, 0.8f) },
                hover = { textColor = new Color(0.9f, 0.9f, 0.9f) },
                active = { textColor = new Color(1.0f, 0.8f, 0.5f) },
                focused = { textColor = Color.yellow }
            };

            foldoutItemStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(20, 0, 2, 2),
                margin = new RectOffset(15, 0, 2, 2),
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f) },
                hover = { textColor = Color.cyan },
                active = { textColor = Color.white },
                focused = { textColor = new Color(1.0f, 0.8f, 0.3f) }
            };
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        class AnimTree
        {
            public AnimNode root;

            public AnimTree()
            {
                root = new AnimNode("root");
            }

            public AnimTree(AnimNode node)
            {
                root = new AnimNode("root", node);
            }

            public AnimTree(List<AnimNode> nodes)
            {
                root = new AnimNode("root", nodes);
            }

            public void AddNode(string path)
            {
                root.AddNode(new List<string>(path.Split("/")));
            }

            public void DrawTree()
            {
                EditorGUILayout.BeginVertical();
                root.DrawTreeRoot();
                EditorGUILayout.EndVertical();
            }

            public StringPair GetPaths()
            {
                return root.GetPathsFromStart();
            }

            public void SetTemplate(GameObject gameObject)
            {
                root.AddTemplate(gameObject);
            }
        }

        class AnimNode
        {
            public string nodeName;
            public string nodeNameChanged;
            public List<AnimNode> nodes;
            public bool foldout = true;
            public static bool noTemplate;

            public GameObject templateObject;

            public AnimNode(string name, AnimNode node)
            {
                this.nodeName = name;
                nodeNameChanged = name;
                this.nodes = new List<AnimNode>() { node };
            }

            public AnimNode(string name, List<AnimNode> nodes)
            {
                this.nodeName = name;
                nodeNameChanged = name;
                this.nodes = nodes;
            }

            public AnimNode(string name)
            {
                this.nodeName = name;
                nodeNameChanged = name;
                this.nodes = new List<AnimNode>();
            }

            public void AddNode(List<string> paths)
            {
                if (paths.Count <= 0)
                {
                    return;
                }

                string top = paths[0];
                paths.RemoveAt(0);

                int index = nodes.FindIndex(node => node.nodeName == top);

                if (paths.Count <= 0 && index < 0)
                {
                    nodes.Add(new AnimNode(top));
                    return;
                }

                if (paths.Count <= 0 && index >= 0)
                {
                    return;
                }

                if (index >= 0)
                {
                    nodes[index].AddNode(paths);
                }
                else
                {
                    nodes.Add(new AnimNode(top));
                }
            }

            public void DrawTree()
            {
                EditorGUILayout.BeginVertical(foldoutItemStyle);

                if (nodes.Count <= 0)
                {
                    // EditorGUILayout.LabelField($"   {nodeName}");
                    ChangeName();
                }
                else
                {
                    //foldout = EditorGUILayout.Foldout(foldout, nodeName, true, foldoutStyle);

                    EditorGUILayout.BeginHorizontal();
                    foldout = EditorGUILayout.Foldout(foldout, GUIContent.none);
                    ChangeName();
                    EditorGUILayout.EndHorizontal();
                }

                if (foldout)
                {
                    foreach (var node in nodes)
                    {
                        node.DrawTree();
                    }
                }

                EditorGUILayout.EndVertical();
            }

            public void DrawTreeRoot()
            {
                EditorGUILayout.BeginVertical(foldoutItemStyle);

                if (nodes.Count <= 0)
                {
                    EditorGUILayout.LabelField($"root");
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    foldout = EditorGUILayout.Foldout(foldout, "root");
                    EditorGUILayout.EndHorizontal();
                }

                if (foldout)
                {
                    foreach (var node in nodes)
                    {
                        node.DrawTree();
                    }
                }

                EditorGUILayout.EndVertical();
            }

            public void ChangeName()
            {
                GUIStyle textFieldStyle = new GUIStyle(EditorStyles.textField)
                {
                    normal = { textColor = noTemplate ? Color.white : templateObject == default ? Color.red : Color.green }, // Change this to any color
                    focused = { textColor = Color.white }, // Ensures color stays the same when focused
                };

                partialMatch |= !noTemplate && templateObject == default;

                nodeNameChanged = EditorGUILayout.TextField(GetName(), textFieldStyle);
            }

            public void ChangeName(string newName)
            {
                nodeNameChanged = newName;
            }

            public string GetName()
            {
                return nodeNameChanged.Length <= 0 ? nodeName : nodeNameChanged;
            }

            public StringPair GetPaths(string currentLocation, string currentLocationChanged)
            {
                StringPair stringPair = new StringPair();

                string newLocation = $"{currentLocation}/{nodeName}";
                string newLocationChanged = $"{currentLocationChanged}/{GetName()}";

                stringPair.original.Add(newLocation);
                stringPair.changed.Add(newLocationChanged);

                if (nodes.Count <= 0)
                {
                    return stringPair;
                }

                foreach (var node in nodes)
                {
                    StringPair pair = node.GetPaths(newLocation, newLocationChanged);

                    stringPair.original.AddRange(pair.original);
                    stringPair.changed.AddRange(pair.changed);
                }

                return stringPair;
            }

            public StringPair GetPathsPre()
            {
                StringPair stringPair = new StringPair();

                string newLocation = $"{nodeName}";
                string newLocationChanged = $"{GetName()}";

                stringPair.original.Add(newLocation);
                stringPair.changed.Add(newLocationChanged);

                if (nodes.Count <= 0)
                {
                    return stringPair;
                }

                foreach (var node in nodes)
                {
                    StringPair pair = node.GetPaths(newLocation, newLocationChanged);

                    stringPair.original.AddRange(pair.original);
                    stringPair.changed.AddRange(pair.changed);
                }

                return stringPair;
            }

            public StringPair GetPathsFromStart()
            {
                StringPair stringPair = new StringPair();

                string newLocation = "";
                string newLocationChanged = "";

                if (nodes.Count <= 0)
                {
                    stringPair.original.Add(newLocation);
                    stringPair.changed.Add(newLocationChanged);

                    return stringPair;
                }

                foreach (var node in nodes)
                {
                    StringPair pair = node.GetPathsPre();

                    stringPair.original.AddRange(pair.original);
                    stringPair.changed.AddRange(pair.changed);
                }

                return stringPair;
            }

            public void AddTemplate(GameObject templateObject)
            {
                this.templateObject = templateObject;

                ChangeName(templateObject.name);

                if (nodes.Count != templateObject.transform.childCount)
                {
                    return;
                }

                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].AddTemplate(templateObject.transform.GetChild(i).gameObject);
                }
            }

            public void AddTemplateTest(GameObject templateObject)
            {
                this.templateObject = templateObject;

                if (nodes.Count != templateObject.transform.childCount)
                {
                    return;
                }

                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].AddTemplateTest(templateObject.transform.GetChild(i).gameObject);
                }
            }

            public void ResetTemplate()
            {
                partialMatch = false;

                this.templateObject = default;

                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].ResetTemplate();
                }
            }
        }

        class StringPair
        {
            public List<string> original;
            public List<string> changed;

            public StringPair()
            {
                original = new List<string>();
                changed = new List<string>();
            }

            public StringPair(List<string> original, List<string> changed)
            {
                this.original = original;
                this.changed = changed;
            }
        }
    }
}