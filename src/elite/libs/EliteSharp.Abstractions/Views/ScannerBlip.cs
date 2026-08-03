// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// One lollipop on the scanner, in the scanner's own units relative to its
/// centre and unclipped - each tier's scanner is a different size, so how far
/// off centre is too far is the view's own business.
/// </summary>
/// <param name="X">Distance right of the scanner's centre.</param>
/// <param name="StickY">
/// The stick's far end - the object's position in the scanner plane.
/// </param>
/// <param name="BlipY">
/// The blip itself, which is the stick's end lifted by the object's height, so
/// the stick's length is what shows how far above or below the plane it is.
/// </param>
/// <param name="Kind">What sort of thing it is, which picks the colour.</param>
public readonly record struct ScannerBlip(float X, float StickY, float BlipY, ShipClass Kind);
