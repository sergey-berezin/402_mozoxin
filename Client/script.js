let childrenSize = document.getElementById("childen_size_text");
let populationSize = document.getElementById("population_size_text");
let mutationSize = document.getElementById("mutation_size_text");
let figuresSizes = document.getElementById("figures_sizes_text");
let canvas = document.getElementById("canvas");
let metricText = document.getElementById("metrics");
let start = document.getElementById("start")
const canvas_context = canvas.getContext("2d");
canvas_context.strokeStyle = "red";

childrenSize.value = "100";
populationSize.value = "500";
mutationSize.value = "1";
figuresSizes.value = "3, 2, 2, 1, 1";

let isRunning = false;


async function startExperiment()
{
    start.disabled = true;
    isRunning = true
    const sizes = figuresSizes.value.split(", ")
    let start_query = new URLSearchParams({
        childrenSize: childrenSize.value,
        populationSize: populationSize.value,
        mutationSize: mutationSize.value,
        figuresAmount: sizes.length
    }).toString()
    for (const size of sizes) {
        start_query += "&figuresSizes=" + size;
    }
    
    let response = await fetch("http://localhost:5276/initial?" + start_query);

    info = await response.json();
    
    let calculation = new Promise(async () => {
        let iteration = 0
        while(isRunning)
        {
            let query = {
                childrenSize: childrenSize.value,
                populationSize: populationSize.value,
                mutationSize: mutationSize.value,
                figuresAmount: sizes.length,
                figuresSizes: sizes,
                population: info.population,
                iterationsCompleted: iteration
            };
            
            let response = await fetch("http://localhost:5276/next", {
                method: 'POST',
                headers: {
                'Content-Type': 'application/json;charset=utf-8',
                },
                body: JSON.stringify(query)
            });
            info = await response.json();
            console.log(info)
            let bestSolution = info.bestSolution;
            metricText.textContent = "Метрика лучшего результата: " + bestSolution.metric;

            let bounds = bestSolution.bounds;

            let sizeX = Math.abs(bounds[0][0] - bounds[1][0]);
            let sizeY = Math.abs(bounds[0][1] - bounds[1][1]);
            if (sizeX > sizeY)
            {
                sizeY = sizeX;
            }
            else
            {
                sizeX = sizeY;
            }
            let height = canvas.offsetHeight;
            let width = canvas.offsetWidth;
            canvas_context.clearRect(0, 0, width, height);
            
            for (let i = 0; i < sizes.length; i++)
            {
                canvas_context.fillRect(
                    (bestSolution.gens[i][0] - bounds[0][0]) / sizeX * width,
                    (bestSolution.gens[i][1] - bounds[0][1]) / sizeY * height,
                    sizes[i] / sizeX * width,
                    sizes[i] / sizeY * height
                );
                
                canvas_context.strokeRect(
                    (bestSolution.gens[i][0] - bounds[0][0]) / sizeX * width,
                    (bestSolution.gens[i][1] - bounds[0][1]) / sizeY * height,
                    sizes[i] / sizeX * width,
                    sizes[i] / sizeY * height
                );
            }

            iteration++;
        }
    });
}

function stopExperiment()
{
    isRunning = false
    start.disabled = false;
}
