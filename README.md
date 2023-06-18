1. Create a Folder called "package".

2. Inside package folder create a new folder called "ProjectData~".

3. From Unity Project Folder, Copy "Assets, Packages, Project Settings" and put it into the folder "ProjectData~".

4. In the folder "Project Settings", Delete "ProjectVersion.txt" file.

5. Create a file "package.json" in the folder "package".
	{
		"name": "com.unity.template.hypercasualui",
		"displayName": "Hypercasual",
		"version": "1.1.0",
		"type": "template",
		"unity": "2019.4",
		"description": "Use this template to create Hypercasual Base.",
		"dependencies": {}
	}

6. Create a tar file by using this command.
	tar czf com.unity.template.hypercasualui-1.1.1.tgz package

7. Paste com.unity.template.hypercasualui-1.1.1.tgz into "/Applications/Unity/2021.3.16f1/Unity.app/Contents/Resources/PackageManager/ProjectTemplates".

8. Restart Unity Hub.