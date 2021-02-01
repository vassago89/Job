This project is split in 3 sections: CondorDriver, CondorViewer and ImageProcessingToolbox.

CondorDriver is a driver for the CONDOR3 - VNN - ICX692 - CL01 visible/IR/IR camera. It starts the camera, grabs images from it, debayers the visible image, then applies auto brightness-contrast-gamma to them using the ImageProcessingToolbox. After that, it emits a signal containing the 3 channels to be caught by the GUI.

CondorViewer is a GUI that gets its images from the CondorDriver. The whole project is only a GUI, plus a streamer (easily identifiable by its file name). It also uses ImageProcessingToolbox to apply false colors to the IR channels of the camera.

ImageProcessingToolbox is just that, a file with a few useful static image processing functions.

The project relies heavily on OpenCV 3.4, as well as Architector, the SDK for the Condor camera.