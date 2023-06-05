// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static w
const canvasBlock = document.getElementById('canvas');
// Создаем сцену
var stage = new Konva.Stage({
    container: 'canvas', // Идентификатор контейнера для сцены
    width: canvasBlock.offsetWidth, // Ширина сцены равная ширине окна
    height: canvasBlock.offsetHeight // Высота сцены равная высоте окна
});

// Создаем слой
var layer = new Konva.Layer();

// Создаем квадрат


// Добавляем слой на сцену
stage.add(layer);

function addRecct(){
    var square = new Konva.Rect({
        x: 20, // Координата X левого верхнего угла квадрата
        y: 20, // Координата Y левого верхнего угла квадрата
        width: 100, // Ширина квадрата
        height: 100, // Высота квадрата// Цвет заливки квадрата
        draggable: true, // Возможность перетаскивания квадрата
        stroke: 'black',
        strokeWidth: 2,
    });

// Добавляем квадрат на слой
    layer.add(square);
}