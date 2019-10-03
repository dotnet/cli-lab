'''
Converts raw blob storage jsons to format usable for the uninstall tool's VS version protection
output format: { vsVersion -> {sdkVersion(s)} }
'''

import json 
from collections import namedtuple
from urllib.request import urlopen
from distutils.version import LooseVersion
import os

blobUrls = ["https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/3.0/releases.json", 
    "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/2.2/releases.json", 
    "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/2.1/releases.json", 
    "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/2.0/releases.json", 
    "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/1.1/releases.json", 
    "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/1.0/releases.json"]

outputFile = os.path.join(os.path.realpath("..\.."), "Resources\VS-SDKVersions.txt")

formattedData = {}
for url in blobUrls:
    origJson = urlopen(url).read()
    origData = json.loads(origJson)["releases"]
    # Gather sdk pairs
    for release in origData:
        bundleList = release.get("sdk")
        sdkVersion = bundleList.get("version").split('-')[0] # don't include any preview strings
        vsVersion = bundleList.get("vs-version")
        if vsVersion is not None and vsVersion is not "" and LooseVersion(sdkVersion) < LooseVersion("3.0.0"):
            vsVersion = vsVersion.split('-')[0]
            if len(vsVersion.split('.')) < 3:
                vsVersion = vsVersion + ".0"
            if formattedData.get(vsVersion) is None or formattedData.get(vsVersion) < sdkVersion:
                formattedData[vsVersion] = sdkVersion

formattedData["0.0.0"] = "0.0.0" # Adding mock data
formattedData["0.0.1"] = "0.0.1" # Adding mock data
formattedData["0.0.2"] = "0.1.0" # Adding mock data
print(formattedData)
json.dump(formattedData, open(outputFile,"w"), cls=json.JSONEncoder)