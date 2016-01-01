using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.PackageManager.GitHub;
using System.Net;
using System;
using System.IO;

namespace Unity.CHIPS.PackageManager.GitHub
{
    public class PackageManagerEditorWindow : EditorWindow
    {
        protected List<List<GUIContent>> m_items;
        protected Vector2 m_scrollVector;
        protected Vector2 m_infoScrollVector;
        protected PackageManager m_packageManager = null;
        protected bool m_initialized;

        // Add menu named "Package Manager" to the "Tools" menu
        [MenuItem ("Tools/Package Manager")]
        static void ShowPackageManager()
        {
            // Get existing open window or if none, make a new one:
            var window = EditorWindow.GetWindow<PackageManagerEditorWindow>();
            window.Show();
        }

        public void Initialize()
        {
            var packageManifests = new List<string>() { "https://github.com/libgit2/libgit2sharp/blob/vNext/.gitignore" };
            m_packageManager = new PackageManager( packageManifests );
            m_packageManager.RefreshPackagManifest ();

            m_items = new List<List<GUIContent>>();

            var rowItems = new List<GUIContent>();
            rowItems.Add( new GUIContent( "Id", "Package Id Tool Tip" ) );
            rowItems.Add( new GUIContent( "Version", "Package Version Tool Tip" ) );
            rowItems.Add( new GUIContent( "Name", "Package Name Tool Tip" ) );
            m_items.Add( rowItems );

            rowItems = new List<GUIContent>();
            rowItems.Add( new GUIContent( "test.id.1" ) );
            rowItems.Add( new GUIContent( "1.0.0" ) );
            rowItems.Add( new GUIContent( "Test Name 1" ) );
            m_items.Add( rowItems );

            rowItems = new List<GUIContent>();
            rowItems.Add( new GUIContent( "test.id.2" ) );
            rowItems.Add( new GUIContent( "1.0.1" ) );
            rowItems.Add( new GUIContent( "Test Name 2" ) );
            m_items.Add( rowItems );

            m_initialized = true;
        }

        void OnGUI ()
        {
            if ( !m_initialized )
            {
                Initialize();
            }

            GUILayout.BeginVertical();
            GUILayout.Label ("Available Packages:", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal(GUI.skin.box);
            m_scrollVector = GUILayout.BeginScrollView(m_scrollVector, false, true);

            GUILayout.BeginVertical();
            if ( m_items != null )
            {
                for (int i = 0; i < m_items.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    for ( int j = 0; j < m_items[0].Count; ++j )
                    {
                        if ( j == 0 )
                        {
                            GUILayout.Label(m_items[i][j], GUI.skin.box, new GUILayoutOption[] { GUILayout.MaxWidth( 200.0f ), GUILayout.MinWidth( 100.0f ) } );
                        }
                        else if ( j == 1 )
                        {
                            GUILayout.Label(m_items[i][j], GUI.skin.box, new GUILayoutOption[] { GUILayout.MaxWidth( 200.0f ), GUILayout.MinWidth( 100.0f ) } );
                        }
                        else if ( j == 2 )
                        {
                            GUILayout.Label(m_items[i][j], GUI.skin.box, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.MinWidth( 150.0f ) });
                        }

                        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                        {
                            // Handle events here
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            // Show Info Window
            GUILayout.Label ("Packages Info:", EditorStyles.boldLabel);
            m_infoScrollVector = GUILayout.BeginScrollView(m_infoScrollVector, false, true, GUILayout.MaxHeight(100.0f));
            GUILayout.Label("", GUI.skin.box, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) } );
            GUILayout.EndScrollView();

            // Show Bottom Buttons
            GUILayout.BeginHorizontal();
            GUILayout.Button( new GUIContent( "Deploy into Project" ) );
            GUILayout.Button( new GUIContent( "Undeploy" ) );
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}