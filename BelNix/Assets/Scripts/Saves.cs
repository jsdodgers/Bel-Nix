using UnityEngine;
using System.Collections;
using System.IO;

public class Saves {

	public static string getSaveRootDirectory() {
		return Application.persistentDataPath + "/Saves/";
	}

	public static string getSaveDirectory(string save) {
		return getSaveRootDirectory() + save;
	}

	public static string getCurrentSaveDirectory() {
		return getSaveDirectory("Current");
	}

	public static bool hasSaveFileNamed(string save) {
		return Directory.Exists(getSaveDirectory(save));
	}

	public static bool hasCurrentSaveFile() {
		return hasSaveFileNamed("Current");
	}

	public static void deleteSaveFileNamed(string save) {
		if (hasSaveFileNamed(save)) {
			removeFilesFromSaveFileNamed(save);
			Directory.Delete(getSaveDirectory(save));
		}
	}

	public static void removeFilesFromSaveFileNamed(string save) {
		DirectoryInfo info = new DirectoryInfo(getSaveDirectory(save));
		foreach (FileInfo file in info.GetFiles()) {
			Debug.Log(file.FullName);
			file.Delete();
		}
		foreach (DirectoryInfo dir in info.GetDirectories()) {
			Debug.Log(dir.FullName);
			dir.Delete(true);
		}
	}

	public static void removeFilesFromCurrentSaveFile() {
		removeFilesFromSaveFileNamed("Current");
	}

	public static void deleteCurrentSaveFile() {
		deleteSaveFileNamed("Current");
	}

	public static void createSaveFileNamed(string save) {
		Directory.CreateDirectory(getSaveDirectory(save));
	}

	public static void createCurrentSaveFile() {
		Directory.CreateDirectory(getCurrentSaveDirectory());
	}

	public static string getCharactersListFilePath() {
		return getCurrentSaveDirectory() + "/Characters.txt";
	}

	public static string getCharacterPath(string characterId) {
		return getCurrentSaveDirectory() + "/" + characterId + ".txt";
	}

	public static string getCharactersString(string characterId) {
		return File.ReadAllText(getCharacterPath(characterId));
	}

	public static string[] getCharacterList() {
		string text = File.ReadAllText(getCharactersListFilePath());
		return text.Split(new char[]{';'});
	}

	public static void deleteCharacter(string characterId) {
		if (File.Exists(getCharacterPath(characterId))) {
			File.Delete(getCharacterPath(characterId));
			string[] characters = getCharacterList();
			File.Delete(getCharactersListFilePath());
			string totes = "";
			foreach (string st in characters) {
				if (st == characterId || st == null || st == "") continue;
//				totes += ";";
				totes += st + ";";
			}
			File.WriteAllText(getCharactersListFilePath(), totes);
		}
	}

	public static void saveCharacter(string characterId, string characterStr) {
		if (File.Exists(getCharacterPath(characterId))) {

		}
		File.WriteAllText(getCharacterPath(characterId), characterStr);
	}

	public static string addCharacter(string character) {
		int currAdd = 0;
		string fileDirectory = getCurrentSaveDirectory() + "/";
		string fileN = System.Guid.NewGuid().ToString();
		string fileN2 = fileN + (currAdd>0?"" +currAdd:"");
		string fileName = fileDirectory + fileN2 + ".txt";
		while(File.Exists(fileName))
		{
			currAdd++;
			fileN2 = fileN + (currAdd>0?"" +currAdd:"");
			fileName = fileDirectory + fileN2 + ".txt";
		}
		
		//		StreamWriter sr = File.CreateText(fileName);
		//		sr.WriteLine(characterStr);
		//		sr.Close();
		//		File.CreateText(fileName);
		if (!Directory.Exists(fileDirectory)) {
			Directory.CreateDirectory(fileDirectory);
		}
		File.WriteAllText(fileName, character);
		
		//	if (!File.Exists(path2)) {
		//		File.CreateText(path2);
		//	}
		File.AppendAllText(getCharactersListFilePath(), fileN2 + ";");
		return fileN2;
	}

	public static void saveAs(string save) {
		deleteSaveFileNamed(save);
		createSaveFileNamed(save);
		DirectoryInfo dir = new DirectoryInfo(getCurrentSaveDirectory());
		if (!dir.Exists) return;
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files) {
			string temppath = Path.Combine(getSaveDirectory(save), file.Name);
			file.CopyTo(temppath, false);
		}
	}
	
	public static void loadSave(string save) {
		deleteCurrentSaveFile();
		createCurrentSaveFile();
		DirectoryInfo dir = new DirectoryInfo(getSaveDirectory(save));
		if (!dir.Exists) return;
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files) {
			file.CopyTo(Path.Combine(getCurrentSaveDirectory(),file.Name), false);
		}
	}

	public static string[] getSaveFiles() {
		string[] s = Directory.GetDirectories(getSaveRootDirectory());
		string[] ans = new string[s.Length-1];
		int n = 0;
		foreach (string ss in s) {
			string[] sss = ss.Split('/');
			if (sss[sss.Length-1]!="Current") {
				ans[n] = sss[sss.Length-1];
				n++;
			}
		}
		return ans;
	}
}
