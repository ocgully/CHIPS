using UnityEngine;
using System.Collections;
using Semver;
using LitJson;
using System.Collections.Generic;

namespace Unity.CHIPS.PackageManager.GitHub
{
    public class PackageManifest
    {
        public Dictionary<string, string> IdToPackageRepositoryMap;
    }

    public static class PackageManifestFactory
    {
        public static PackageManifest Create( string json )
        {
            PackageManifest manifest = new PackageManifest();
            manifest.IdToPackageRepositoryMap = JsonMapper.ToObject<Dictionary<string, string>>( json ); 
            return manifest;
        }
    }

    public class Package
    {
        public string PackageRepositoryURL;
        public string Id;
        public string Name;
        public SemVersion Version;
        public string Description;
        public List<string> RequiredPackages;
        public List<string> LibraryFiles;
        public List<string> SourceFiles;

        public void Deploy()
        {
        }

        public void Undeploy()
        {
            // If source code, do a check against the stored version, to verify that no local changes have been made (files modified, removed, etc.)
            // Then warn the user that there have been modifications to the source.
        }
    }

    public static class PackageFactory
    {
        public static Package Create( string json )
        {
            Package package = JsonMapper.ToObject<Package>(json);
            return package;
        }
    }

    public class PackageManager
    {
        protected const string PACKAGE_INFO_FILE = "PackageInfo.json";
        protected List<string> m_packageManifestURLs;
        protected PackageManifest m_packageManifest = PackageManifestFactory.Create("");
        protected Dictionary<string, Package> m_packagesAvailable = new Dictionary<string, Package>();
        protected Dictionary<string, Package> m_packagesDeployed = new Dictionary<string, Package>();
        DownloadManager m_downloadManager = new DownloadManager( 4 );

    	public PackageManager( List<string> packageManifestURLs )
    	{
            m_packageManifestURLs = packageManifestURLs;
    	}

    	public void RefreshPackagManifest()
    	{
            m_packagesAvailable.Clear();
            m_packagesDeployed.Clear();

            m_downloadManager.StopAllDownloads();
            //m_downloadManager.DownloadFile ("http://www.google.com/index.html", Application.dataPath + "/test.txt" );
            m_downloadManager.DownloadData("https://raw.githubusercontent.com/cocos2d/cocos2d-x/v3/download-deps.py", ( bytes ) => {
                Debug.Log( System.Text.Encoding.Default.GetString( bytes ) );
            });

        }

//        protected IEnumerator PopulatePackageInformation()
//        {
//            // Get the master package manifest
//            foreach( var manifestURL in m_packageManifestURLs )
//            {
//                WWW wwwRequest = new WWW( manifestURL );
//                yield return wwwRequest;
//
//                if ( wwwRequest.isDone )
//                {
//                    string json = System.Text.Encoding.UTF8.GetString( wwwRequest.bytes );
//
//                    Debug.Log( json );
//                    m_packageManifest = PackageManifestFactory.Create( json );
//
//                    foreach ( var kvp in m_packageManifest.IdToPackageRepositoryMap )
//                    {
//                        if ( !string.IsNullOrEmpty( kvp.Value ) )
//                        {
//                            yield return PopulatePackageInformation( kvp.Value );
//                        }
//                    }
//                }
//            }
//        }
//
//        protected IEnumerator PopulatePackageInformation( string packageRepositoryURL )
//        {
//            // Get package specific information
//            WWW wwwRequest = new WWW( string.Format( "{0}{1}", packageRepositoryURL, PACKAGE_INFO_FILE ) );
//            yield return wwwRequest;
//
//            if ( wwwRequest.isDone )
//            {
//                string json = System.Text.Encoding.UTF8.GetString( wwwRequest.bytes );
//
//                Debug.Log( json );
//
//                Package package = PackageFactory.Create( json );
//                m_packagesAvailable.Add( package.Id, package );
//            }
//        }
//
//        public void EnumerateDeployedPackages()
//        {
//            // Iterate through project and locate all packages, create a package that provides the information 
//
//            // m_packagesDeployed
//        }
    }
}